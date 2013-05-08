using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Infrastructure;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Expressions
{
    public sealed class ConstrantEvaluatorTests
    {
        // ReSharper disable ConvertToConstant.Local

        [Fact]
        public void ClosureOnLocalVariable()
        {
            // Arrange
            string value = "String value";
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => value;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(value, ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnSubstringOfLocalVariable()
        {
            // Arrange
            string value = "String value";
            var evaluator = new ExpressionEvaluator();
            const int substringIndex = 7;
            Expression<Func<string>> lambda = () => value.Substring(substringIndex);

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(value.Substring(substringIndex), ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnEnumValue()
        {
            // Arrange
            var country = Countries.Finland;
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => country.ToString();

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(country.ToString(), ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnClassConstant()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => string.Empty;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(string.Empty, ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnLocalVariableTwice()
        {
            // Arrange
            string value = "String value";
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => value;

            // Act
            Expression result1 = evaluator.Evaluate(lambda.Body);
            Expression result2 = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result1);
            Assert.IsType<ConstantExpression>(result2);
            Assert.Equal(value, ((ConstantExpression) result1).Value);
            Assert.Equal(value, ((ConstantExpression) result2).Value);
        }

        // ReSharper restore ConvertToConstant.Local

        public void ClosureOnVariableWithProperty()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            var country = new Country {Continent = "abc"};
            Expression<Func<string>> lambda = () => country.Continent;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(country.Continent, ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnVariableWithNotAutoProperty()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            var user = new Entity();
            Expression<Func<byte[]>> lambda = () => user.NotAutoProperty;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(user.NotAutoProperty, ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnVariableWithField()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            var user = new Entity {PublicField = 123};
            Expression<Func<int>> lambda = () => user.PublicField;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(user.PublicField, ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnLocalConstant()
        {
            // Arrange
            const long value = 0xfefefe;
            var evaluator = new ExpressionEvaluator();
            Expression<Func<long>> lambda = () => value;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(value, ((ConstantExpression) result).Value);
        }

        [Fact]
        public void ClosureOnForeachLoopValue()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            var data = new byte[] {0x11, 0x22, 0x33};
            var values = new List<object>(data.Length);

            // Act
            // ReSharper disable AccessToForEachVariableInClosure
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (byte value in data)
            {
                Expression<Func<byte>> lambda = () => value;
                var result = (ConstantExpression) evaluator.Evaluate(lambda.Body);
                values.Add(result.Value);
            }
            // ReSharper restore LoopCanBeConvertedToQuery
            // ReSharper restore AccessToForEachVariableInClosure

            // Assert
            Assert.Equal(data.Length, values.Count);
            foreach (byte value in data)
            {
                Assert.Contains(value, values);
            }
        }

        [Fact]
        public void ClosureOnFunctionValue()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => GetTestString();

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(GetTestString(), ((ConstantExpression) result).Value);
        }

        private string GetTestString()
        {
            return "Test string";
        }

        [Fact(Skip = "Only for performance measurement")]
        public void MeasureEvaluationTime()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            var country = new Country {Continent = "abc"};
            Expression<Func<string>> lambda = () => country.Continent;
            var stopwatch = new Stopwatch();

            // Act
            evaluator.Evaluate(lambda.Body);
            stopwatch.Start();
            for (int i = 0; i < 60000000; i++)
            {
                country.Continent = i.ToString(CultureInfo.InvariantCulture);
                evaluator.Evaluate(lambda.Body);
            }
            stopwatch.Stop();
        }
    }
}