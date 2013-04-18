using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public class QueryTranslatorTests
    {
        private IQueryable<T> GetQueryable<T>()
        {
            return new List<T>().AsQueryable();
        }

        // ReSharper disable ConvertToConstant.Local

        [Fact]
        public void TranslateExpressionWithInvokations()
        {
            // Arrange
            var country1 = new Country {Continent = "Europe"};
            var country2 = new Country {Continent = "South America", Name = "Brazil"};

            Expression<Func<Country, bool>> predicate = country => country.Continent == country1.Continent;
            predicate = Or(predicate, country => country.Continent == country2.Continent);
            predicate = And(predicate, country => country.Name == country2.Name);

            IQueryable<Country> expression = GetQueryable<Country>().Where(predicate);

            var queryTranslator = new QueryTranslator(new Dictionary<string, string>());

            // Act
            IDictionary<QuerySegment, string> result = queryTranslator.Translate(expression.Expression);

            // Assert
            Assert.NotNull(result);
        }

        // ReSharper restore ConvertToConstant.Local

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
    }
}