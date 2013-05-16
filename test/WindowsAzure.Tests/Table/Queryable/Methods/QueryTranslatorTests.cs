using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public class QueryTranslatorTests
    {
        private const string ResultFormat = "{0} iterations took {1} ms";
        private const int IteractionsCount = 1000000;

        private readonly Dictionary<string, string> _nameChanges = new Dictionary<string, string>
            {
                {"Continent", "PartitionKey"},
                {"Name", "RowKey"}
            };

        private IQueryable<T> GetQueryable<T>()
        {
            return new List<T>().AsQueryable();
        }

        [Fact]
        public void TranslateExpressionWithProjections()
        {
            // Arrange
            var query = GetQueryable<Country>().Select(p => new {p.Name, p.Continent});
            var translator = new QueryTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act
            translator.Translate(query.Expression, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Equal(2, translation.TableQuery.SelectColumns.Count);
            Assert.Contains("PartitionKey", translation.TableQuery.SelectColumns);
            Assert.Contains("RowKey", translation.TableQuery.SelectColumns);
        }

        [Fact]
        public void RetrieveProjectedData()
        {
            // Arrange
            var countries = new List<Country>
                {
                    new Country {Continent = "continent1", Name = "name1"},
                    new Country {Continent = "continent2", Name = "name2"}
                };
            IQueryable<ProjectionClass> query = GetQueryable<Country>()
                .Select(p => new ProjectionClass {Name = p.Name, Continent = p.Continent});

            var translator = new QueryTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate(query.Expression, translation);

            Assert.NotNull(translation.PostProcessing);

            object result = translation.PostProcessing.DynamicInvoke(countries.AsQueryable());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<object>>(result);

            List<ProjectionClass> entities = ((IEnumerable<ProjectionClass>) result).ToList();
            IEnumerable<string> names = entities.Select(p => p.Name).ToList();
            IEnumerable<string> continents = entities.Select(p => p.Continent).ToList();

            Assert.Contains("name1", names);
            Assert.Contains("name2", names);
            Assert.Contains("continent1", continents);
            Assert.Contains("continent2", continents);
        }

        [Fact(Skip = "Only for performance measurement")]
        public void QueryTranslatorPerformanceMeasurement()
        {
            // Arrange
            var list = new List<Guid>
                {
                    new Guid("48ed2917-f7d4-4383-aa29-4062f1296bbc"),
                    new Guid("8a024e77-4f06-49d9-9d46-9a0e59d74fcd")
                };

            IQueryable<Country> query = GetQueryable<Country>()
                .Where(
                    p => p.Formed > new DateTime(123, 1, 1) &&
                         (p.PresidentsCount < 10 && list.Contains(p.Id) ||
                          p.Population < 10000000 && p.PresidentsCount > 10 && p.IsExists));

            var queryTranslator = new QueryTranslator(_nameChanges);
            TranslationResult translationResult = null;

            // Act
            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < IteractionsCount; i++)
            {
                translationResult = new TranslationResult();
                queryTranslator.Translate(query.Expression, translationResult);
            }

            stopwatch.Stop();

            // Assert
            Assert.NotNull(translationResult);
            Assert.Equal(
                "Formed gt datetime'0123-01-01T00:00:00' and ((PresidentsCount lt 10 and (Id eq guid'48ed2917-f7d4-4383-aa29-4062f1296bbc' or Id eq guid'8a024e77-4f06-49d9-9d46-9a0e59d74fcd')) or ((Population lt 10000000L and PresidentsCount gt 10) and IsExists))",
                translationResult.TableQuery.FilterString);
            Console.WriteLine(ResultFormat, IteractionsCount, stopwatch.ElapsedMilliseconds);
        }

        // ReSharper disable ConvertToConstant.Local

        [Fact]
        public void TranslateExpressionWithInvocations()
        {
            // Arrange
            var country1 = new Country {Continent = "Europe"};
            var country2 = new Country {Continent = "South America", Name = "Brazil"};

            Expression<Func<Country, bool>> predicate = country => country.Continent == country1.Continent;
            predicate = Or(predicate, country => country.Continent == country2.Continent);
            predicate = And(predicate, country => country.Name == country2.Name);

            IQueryable<Country> query = GetQueryable<Country>().Where(predicate);

            var translator = new QueryTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act
            translator.Translate(query.Expression, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("(PartitionKey eq 'Europe' or PartitionKey eq 'South America') and RowKey eq 'Brazil'", translation.TableQuery.FilterString);
        }

        // ReSharper restore ConvertToConstant.Local

        [Fact]
        public void TranslateExpressionWithNotSupportedMethod()
        {
            // Arrange
            var query = GetQueryable<Country>().TakeWhile(p => p.Name != string.Empty);
            var translator = new QueryTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act
            Assert.Throws<NotSupportedException>(() => translator.Translate(query.Expression, translation));

            // Assert
            Assert.NotNull(translation.TableQuery);
        }

        public static Expression<Func<T, bool>> Or<T>(Expression<Func<T, bool>> expr1,
                                                      Expression<Func<T, bool>> expr2)
        {
            InvocationExpression invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>
                (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1,
                                                       Expression<Func<T, bool>> expr2)
        {
            InvocationExpression invokedExpr = Expression.Invoke(expr2, expr1.Parameters);
            return Expression.Lambda<Func<T, bool>>
                (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }

        internal class ProjectionClass
        {
            public string Continent;
            public string Name;
        }
    }
}