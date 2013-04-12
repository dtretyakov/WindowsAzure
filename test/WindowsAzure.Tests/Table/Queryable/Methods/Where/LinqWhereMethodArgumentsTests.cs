using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods.Where
{
    public sealed class LinqWhereMethodArgumentsTests
    {
        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public LinqWhereMethodArgumentsTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        [Fact]
        public void BinaryQueryWithEqualBooleanValueTest()
        {
            // Arrange
            const bool value = false;
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists == value);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("IsExists eq false", result[translator.QuerySegment]);
        }

        [Fact]
        public void BinaryQueryWithNotEqualBooleanValueTest()
        {
            // Arrange
            const bool value = false;
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists != value);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("IsExists ne false", result[translator.QuerySegment]);
        }

        [Fact]
        public void UnaryQueryWithBooleanValueTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.IsExists);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("IsExists", result[translator.QuerySegment]);
        }

        [Fact]
        public void UnaryQueryWithInversedBooleanValueTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => !p.IsExists);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("not IsExists", result[translator.QuerySegment].Trim());
        }
    }
}