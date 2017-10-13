using System;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public sealed class TakeTests
    {
        private static IQueryable<Country> GetQueryable()
        {
            return new EnumerableQuery<Country>(new Country[] {});
        }

        [Fact]
        public void TakeOneEntityTest()
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
        public void TakeOneEntityAfterWhereMethodTest()
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

        [Fact]
        public void TakeWithInvalidMethod()
        {
            // Arrange
            var translator = new TakeTranslator();
            IQueryable<Country> query = GetQueryable().Where(p => p.Name == string.Empty);
            var translation = new TranslationResult();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => translator.Translate((MethodCallExpression) query.Expression, translation));

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Null(translation.TableQuery.TakeCount);
        }
    }
}