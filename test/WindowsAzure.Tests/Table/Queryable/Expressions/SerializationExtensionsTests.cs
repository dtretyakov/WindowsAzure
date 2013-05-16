using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using WindowsAzure.Table.Queryable.Expressions.Infrastructure;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Expressions
{
    public sealed class SerializationExtensionsTests
    {
        [Fact]
        public void SerializeNullValueTest()
        {
            // Arrange
            const string value = null;
            ConstantExpression constant = Expression.Constant(value);

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, "null");
        }

        [Fact]
        public void SerializeStringValueTest()
        {
            // Arrange
            const string value = "String";
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format("'{0}'", value));
        }

        [Fact]
        public void SerializeDateTimeValueTest()
        {
            // Arrange
            DateTime value = DateTime.UtcNow;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format("datetime'{0}'", XmlConvert.ToString(value, XmlDateTimeSerializationMode.RoundtripKind)));
        }

        [Fact]
        public void SerializeDateTimeOffsetValueTest()
        {
            // Arrange
            DateTimeOffset value = DateTime.UtcNow;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format("datetime'{0}'", XmlConvert.ToString(value.DateTime, XmlDateTimeSerializationMode.RoundtripKind)));
        }

        [Fact]
        public void SerializeBooleanValueTest()
        {
            // Arrange
            const bool value = true;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, value.ToString().ToLowerInvariant());
        }

        [Fact]
        public void SerializeInt32ValueTest()
        {
            // Arrange
            const Int32 value = 555;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format(CultureInfo.InvariantCulture, "{0}", value));
        }

        [Fact]
        public void SerializeInt64ValueTest()
        {
            // Arrange
            const Int64 value = 0xfefefe;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format(CultureInfo.InvariantCulture, "{0}L", value));
        }

        [Fact]
        public void SerializeSingleValueTest()
        {
            // Arrange
            const Single value = 0.3f;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format(CultureInfo.InvariantCulture, "{0:#.0#}", value));
        }

        [Fact]
        public void SerializeDoubleValueTest()
        {
            // Arrange
            const Double value = 0.3;
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format(CultureInfo.InvariantCulture, "{0:#.0#}", value));
        }

        [Fact]
        public void SerializeGuidValueTest()
        {
            // Arrange
            Guid value = Guid.NewGuid();
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result, string.Format(CultureInfo.InvariantCulture, "guid'{0}'", value));
        }

        [Fact]
        public void SerializeBytesValueTest()
        {
            // Arrange
            var value = new Byte[] {0x11, 0x22, 0x33};
            ConstantExpression constant = Expression.Constant(value, value.GetType());

            // Act
            string result = constant.Serialize();

            // Assert
            Assert.NotNull(result);

            var stringBuilder = new StringBuilder("X'");

            foreach (byte num in value)
            {
                stringBuilder.AppendFormat("{0:x2}", num);
            }

            stringBuilder.Append("'");

            Assert.Equal(result, stringBuilder.ToString());
        }

        [Fact]
        public void SerializeCountryValueTest()
        {
            // Arrange
            var value = new Country();
            ConstantExpression constant = Expression.Constant(value, value.GetType());
            string result = null;

            // Act
            Assert.Throws<NotSupportedException>(() => result = constant.Serialize());

            // Assert
            Assert.Null(result);
        }
    }
}