using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public sealed class TakeTranslatorTests
    {
        private static IQueryable<Country> GetQueryable()
        {
            return new EnumerableQuery<Country>(new Country[] {});
        }

        [Fact]
        public void LinqTakeOneEntityTest()
        {
            // Arrange
            const int count = 435435;
            IQueryable<Country> query = GetQueryable().Take(count);
            var translator = new TakeTranslator();
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Expression, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.TakeCount);
            Assert.Equal(count, translation.TableQuery.TakeCount);
        }

        [Fact]
        public void LinqTakeOneEntityAfterWhereMethodTest()
        {
            // Arrange
            const int count = 555;
            IQueryable<Country> query = GetQueryable()
                .Where(p => p.Continent == "Europe").Take(count);
            var translator = new TakeTranslator();
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Expression, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.TakeCount);
            Assert.Equal(count, translation.TableQuery.TakeCount);
        }
    }
}