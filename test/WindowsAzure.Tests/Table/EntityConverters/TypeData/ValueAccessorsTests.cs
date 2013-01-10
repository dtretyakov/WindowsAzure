using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class ValueAccessorsTests
    {
        [Fact]
        public void CreatePublicFieldValueAccessorTest()
        {
            // Arrange
            FieldInfo field = typeof (User).GetField("PublicField");

            // Act
            var valueAccessor = new FieldValueAccessor<User>(field);

            // Assert
            Assert.Equal(valueAccessor.Name, "PublicField");
            Assert.Equal(valueAccessor.Type, field.FieldType);
        }

        [Fact]
        public void SetPublicFieldValueTest()
        {
            // Arrange
            FieldInfo field = typeof (User).GetField("PublicField");
            var valueAccessor = new FieldValueAccessor<User>(field);
            var user = new User {PublicField = 123};

            // Act
            valueAccessor.SetValue(user, 222);

            // Assert
            Assert.Equal(user.PublicField, 222);
        }

        [Fact]
        public void GetPublicFieldValueTest()
        {
            // Arrange
            FieldInfo field = typeof (User).GetField("PublicField");
            var valueAccessor = new FieldValueAccessor<User>(field);
            var user = new User {PublicField = 123};

            // Act
            object value = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(value, 123);
        }
    }
}