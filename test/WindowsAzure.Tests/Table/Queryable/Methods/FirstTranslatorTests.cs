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
    public sealed class FirstTranslatorTests
    {
        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public FirstTranslatorTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        [Fact]
        public void LinqFirstClause()
        {
            // Arrange
            var translator = new FirstTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.First(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate((MethodCallExpression) query.Body, translation);

            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        // ReSharper disable ReplaceWithSingleCallToFirst

        [Fact]
        public void LinqFirstAfterWhereClause()
        {
            // Arrange
            var translator = new FirstTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.Where(p => !p.IsExists).First();
            var translation = new TranslationResult();

            // Act && Assert
            translator.Translate((MethodCallExpression) query.Body, translation);

            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        // ReSharper restore ReplaceWithSingleCallToFirst
    }
}