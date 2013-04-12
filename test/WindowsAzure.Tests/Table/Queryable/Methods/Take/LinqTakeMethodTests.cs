using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods.Take
{
    public sealed class LinqTakeMethodTests
    {
        private IQueryable<Country> GetQueryable()
        {
            return new EnumerableQuery<Country>(new Country[] {});
        }

        [Fact]
        public void LinqTakeOneEntityTest()
        {
            // Arrange
            const int count = 435435;
            IQueryable<Country> queryable = GetQueryable();
            IQueryable<Country> query = queryable.Take(count);
            var translator = new TakeTranslator();

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate(
                (MethodCallExpression) query.Expression,
                new Dictionary<string, string>());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal(count.ToString(CultureInfo.InvariantCulture), result[QuerySegment.Top]);
        }

        [Fact]
        public void LinqTakeOneEntityAfterWhereMethodTest()
        {
            // Arrange
            const int count = 555;
            IQueryable<Country> queryable = GetQueryable();
            IQueryable<Country> query = queryable.Where(p => p.Continent == "Europe").Take(count);
            var translator = new TakeTranslator();

            // Act
            IDictionary<QuerySegment, string> result = translator.Translate(
                (MethodCallExpression) query.Expression,
                new Dictionary<string, string>());

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ContainsKey(translator.QuerySegment));
            Assert.Equal(count.ToString(CultureInfo.InvariantCulture), result[QuerySegment.Top]);
        }
    }
}