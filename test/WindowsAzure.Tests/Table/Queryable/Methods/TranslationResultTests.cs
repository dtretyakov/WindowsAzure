using System;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public sealed class TranslationResultTests
    {
        [Fact]
        public void CreateTranslationResult()
        {
            // Act
            var result = new TranslationResult();

            // Assert
            Assert.Null(result.PostProcessing);
            Assert.NotNull(result.TableQuery);
            Assert.Null(result.TableQuery.FilterString);
            Assert.Null(result.TableQuery.TakeCount);
            Assert.Null(result.TableQuery.SelectColumns);
        }

        [Fact]
        public void AddFilter()
        {
            // Arrange
            var result = new TranslationResult();
            const string filter = "name eq '123'";

            // Act
            result.AddFilter(filter);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(filter, result.TableQuery.FilterString);
        }

        [Fact]
        public void AddFilterTwice()
        {
            // Arrange
            var result = new TranslationResult();
            const string filter = "name eq '123'";

            // Act
            result.AddFilter(filter);
            result.AddFilter(filter);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(string.Format("{0} and {0}", filter), result.TableQuery.FilterString);
        }

        [Fact]
        public void AddFilterThreeTimes()
        {
            // Arrange
            var result = new TranslationResult();
            const string filter = "name eq '123'";

            // Act
            result.AddFilter(filter);
            result.AddFilter(filter);
            result.AddFilter(filter);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(string.Format("({0} and {0}) and {0}", filter), result.TableQuery.FilterString);
        }

        [Fact]
        public void AddFiltersWithSimpleAndComplexExpression()
        {
            // Arrange
            var result = new TranslationResult();
            const string simple = "name eq '333'";
            const string complex = "name eq '123' or name eq '222'";

            // Act
            result.AddFilter(simple);
            result.AddFilter(complex);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(string.Format("{0} and ({1})", simple, complex), result.TableQuery.FilterString);
        }

        [Fact]
        public void AddFiltersWithComplexAndComplexExpression()
        {
            // Arrange
            var result = new TranslationResult();
            const string filter = "name eq '123' or name eq '222'";

            // Act
            result.AddFilter(filter);
            result.AddFilter(filter);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(string.Format("({0}) and ({1})", filter, filter), result.TableQuery.FilterString);
        }

        [Fact]
        public void AddNullFilter()
        {
            // Arrange
            var result = new TranslationResult();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => result.AddFilter(null));
        }

        [Fact]
        public void AddTop()
        {
            // Arrange
            var result = new TranslationResult();
            const int count = 5;

            // Act
            result.AddTop(count);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(count, result.TableQuery.TakeCount);
        }

        [Fact]
        public void AddNegativeTop()
        {
            // Arrange
            var result = new TranslationResult();

            // Act && Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => result.AddTop(-2));
        }

        [Fact]
        public void AddColumn()
        {
            // Arrange
            var result = new TranslationResult();
            const string column = "column";

            // Act
            result.AddColumn(column);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.NotNull(result.TableQuery.SelectColumns);
            Assert.Equal(1, result.TableQuery.SelectColumns.Count);
            Assert.Equal(column, result.TableQuery.SelectColumns[0]);
        }

        [Fact]
        public void AddColumnTwice()
        {
            // Arrange
            var result = new TranslationResult();
            const string column = "column";

            // Act
            result.AddColumn(column);
            result.AddColumn(column);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.NotNull(result.TableQuery.SelectColumns);
            Assert.Equal(2, result.TableQuery.SelectColumns.Count);
            Assert.Equal(column, result.TableQuery.SelectColumns[0]);
            Assert.Equal(column, result.TableQuery.SelectColumns[1]);
        }

        [Fact]
        public void AddNullColumn()
        {
            // Arrange
            var result = new TranslationResult();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => result.AddColumn(null));
        }

        [Fact]
        public void AddPostProcessing()
        {
            // Arrange
            var result = new TranslationResult();
            LambdaExpression expression = Expression.Lambda(Expression.Constant(2));

            // Act
            result.AddPostProcesing(expression);

            // Assert
            Assert.NotNull(result.TableQuery);
            Assert.Equal(expression.Compile().DynamicInvoke(), result.PostProcessing.DynamicInvoke());
        }

        [Fact]
        public void AddTwoPostProcessing()
        {
            // Arrange
            var result = new TranslationResult();
            Expression<Func<int>> expression1 = () => 2;
            Expression<Func<int, int>> expression2 = (val) => val + 3;

            // Act
            result.AddPostProcesing(expression1);
            result.AddPostProcesing(expression2);

            // Assert
            Assert.NotNull(result.TableQuery);
            var etalon = expression2.Compile().DynamicInvoke(expression1.Compile().DynamicInvoke());
            Assert.Equal(etalon, result.PostProcessing.DynamicInvoke());
        }

        [Fact]
        public void AddNullPostProcessingExpression()
        {
            // Arrange
            var result = new TranslationResult();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => result.AddPostProcesing(null));
        }
    }
}