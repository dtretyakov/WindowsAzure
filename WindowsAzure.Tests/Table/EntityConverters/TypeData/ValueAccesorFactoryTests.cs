using System;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class ValueAccesorFactoryTests
    {
        [Fact]
        public void CreateValueAccessorFromPropertyInfo()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof (Entity).GetProperty("PublicProperty");

            // Act
            IValueAccessor<Entity> valueAccessor = ValueAccessorFactory.Create<Entity>(propertyInfo);

            // Assert
            Assert.NotNull(valueAccessor);
            Assert.Equal(valueAccessor.Name, propertyInfo.Name);
            Assert.Equal(valueAccessor.Type, propertyInfo.PropertyType);
        }

        [Fact]
        public void CreateValueAccessorFromFieldAccessor()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (Entity).GetField("PublicField");

            // Act
            IValueAccessor<Entity> valueAccessor = ValueAccessorFactory.Create<Entity>(fieldInfo);

            // Assert
            Assert.NotNull(valueAccessor);
            Assert.Equal(valueAccessor.Name, fieldInfo.Name);
            Assert.Equal(valueAccessor.Type, fieldInfo.FieldType);
        }

        [Fact]
        public void CreateValueAccessorFromInvalidMember()
        {
            // Arrange
            MethodInfo methodInfo = typeof (Entity).GetMethod("PublicMethod");

            // Act && Assert
            Assert.Throws<NotSupportedException>(() => ValueAccessorFactory.Create<Entity>(methodInfo));
        }

        [Fact]
        public void CreateValueAccessorFromNullParameter()
        {
            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => ValueAccessorFactory.Create<Entity>(null));
        }
    }
}