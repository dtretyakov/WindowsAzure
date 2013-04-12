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
    public sealed class LinqWhereMethodWithDifferentTypesTests
    {
        private const string Germany = "Germany";
        private const string Europe = "Europe";
        private const string Id = "829ea8b2-3bd5-45a4-8b54-533c69e608d7";

        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public LinqWhereMethodWithDifferentTypesTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        [Fact]
        public void UseWhereOnPartitionKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Continent == Europe);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("PartitionKey eq 'Europe'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == Germany);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("RowKey eq 'Germany'", result[translator.QuerySegment]);
        }

        // ReSharper disable StringCompareToIsCultureSpecific

        [Fact]
        public void UseCompareToInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator();

            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name.CompareTo("F") >= 0 && p.Name.CompareTo("G") <= 0);


            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", result[translator.QuerySegment]);
        }

        // ReSharper restore StringCompareToIsCultureSpecific

        [Fact]
        public void UseCompareInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query =
                () => _countries.Where(p => String.Compare(p.Name, "F", StringComparison.Ordinal) >= 0 && String.Compare(p.Name, "G", StringComparison.Ordinal) <= 0);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseCompareOrdinalInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => String.CompareOrdinal(p.Name, "F") >= 0 && String.CompareOrdinal(p.Name, "G") <= 0);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("RowKey ge 'F' and RowKey le 'G'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseEnumValueInWhereOnRowKeyTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Name == Countries.Germany.ToString());

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("RowKey eq 'Germany'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnDoubleTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Area < 350000);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("Area lt 350000.0", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnBytesTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.TopSecretKey == new byte[] {0xff, 0xee, 0xdd});

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("TopSecretKey eq X'ffeedd'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnDateTimeTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Formed < new DateTime(1800, 1, 1));

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("Formed lt datetime'1800-01-01T00:00:00'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnGuidTest()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Id == new Guid(Id));

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("Id eq guid'829ea8b2-3bd5-45a4-8b54-533c69e608d7'", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnBooleanTest()
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
        public void UseWhereOnInt64Test()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.Population >= 80000000L);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("Population ge 80000000L", result[translator.QuerySegment]);
        }

        [Fact]
        public void UseWhereOnInt32Test()
        {
            // Arrange
            var translator = new WhereTranslator();
            Expression<Func<IQueryable<Country>>> query = () => _countries.Where(p => p.PresidentsCount <= 10);

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate((MethodCallExpression) query.Body, _nameChanges);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal("PresidentsCount le 10", result[translator.QuerySegment]);
        }
    }
}