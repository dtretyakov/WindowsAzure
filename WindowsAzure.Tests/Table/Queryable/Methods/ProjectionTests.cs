using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public class ProjectionTests
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private readonly Dictionary<string, string> _nameChanges;

        public ProjectionTests()
        {
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        private static IQueryable<Country> GetQueryable()
        {
            return new EnumerableQuery<Country>(new Country[] {});
        }

        private static IEnumerable<Country> GetList()
        {
            return new List<Country>
                {
                    new Country
                        {
                            Area = 357021,
                            Continent = "Europe",
                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                            Formed = new DateTime(1871, 1, 18),
                            Id = Guid.NewGuid(),
                            IsExists = true,
                            Name = Germany,
                            Population = 81799600,
                            PresidentsCount = 11
                        },
                    new Country
                        {
                            Area = 505992,
                            Continent = "Europe",
                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                            Formed = new DateTime(1812, 1, 1),
                            Id = Guid.NewGuid(),
                            IsExists = false,
                            Name = Spain,
                            Population = 47190493,
                            PresidentsCount = 8
                        }
                };
        }

        [Fact]
        public void LinqSelectWithProjectionClass()
        {
            // Arrange
            IQueryable<ProjectionResult> query = GetQueryable()
                .Select(p => new ProjectionResult {Name = p.Name, Continent = p.Continent, Area = p.Area});
            var translator = new SelectTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate((MethodCallExpression) query.Expression, translation);

            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.SelectColumns);
            Assert.Equal(3, translation.TableQuery.SelectColumns.Count);
            Assert.Contains("PartitionKey", translation.TableQuery.SelectColumns);
            Assert.Contains("RowKey", translation.TableQuery.SelectColumns);
            Assert.Contains("Area", translation.TableQuery.SelectColumns);

            object result = translation.PostProcessing.DynamicInvoke(GetList().AsQueryable());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<ProjectionResult>>(result);

            List<ProjectionResult> entities = ((IEnumerable<ProjectionResult>) result).ToList();
            IEnumerable<string> names = entities.Select(p => p.Name).ToList();

            Assert.Contains(Germany, names);
            Assert.Contains(Spain, names);
        }

        [Fact]
        public void LinqSelectWithPrivateProjectionClass()
        {
            // Arrange
            IQueryable<InternalProjectionResult> query = GetQueryable()
                .Select(p => new InternalProjectionResult {Name = p.Name, Continent = p.Continent});
            var translator = new SelectTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate((MethodCallExpression) query.Expression, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.SelectColumns);
            Assert.Equal(2, translation.TableQuery.SelectColumns.Count);
            Assert.Contains("PartitionKey", translation.TableQuery.SelectColumns);
            Assert.Contains("RowKey", translation.TableQuery.SelectColumns);

            object result = translation.PostProcessing.DynamicInvoke(GetList().AsQueryable());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<InternalProjectionResult>>(result);

            List<InternalProjectionResult> entities = ((IEnumerable<InternalProjectionResult>) result).ToList();
            IEnumerable<string> names = entities.Select(p => p.Name).ToList();

            Assert.Contains(Germany, names);
            Assert.Contains(Spain, names);
        }

        [Fact]
        public void LinqSelectWithAnonymousType()
        {
            // Arrange
            var query = GetQueryable().Select(p => new {p.Name, p.Continent});
            var translator = new SelectTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate((MethodCallExpression) query.Expression, translation);

            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.SelectColumns);
            Assert.Equal(2, translation.TableQuery.SelectColumns.Count);
            Assert.Contains("PartitionKey", translation.TableQuery.SelectColumns);
            Assert.Contains("RowKey", translation.TableQuery.SelectColumns);

            object result = translation.PostProcessing.DynamicInvoke(GetList().AsQueryable());

            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<object>>(result);
        }

        [Fact]
        public void LinqSelectWithInvalidMethod()
        {
            // Arrange
            IQueryable<Country> query = GetQueryable().Where(p => p.Name == string.Empty);
            var translator = new SelectTranslator(_nameChanges);
            var translation = new TranslationResult();

            // Act && Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => translator.Translate((MethodCallExpression) query.Expression, translation));

            Assert.NotNull(translation.TableQuery);
            Assert.Null(translation.TableQuery.SelectColumns);
        }

        private class InternalProjectionResult
        {
            public string Name { get; set; }
            public string Continent { get; set; }
        }
    }
}