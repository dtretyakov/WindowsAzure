using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class PropertyValueAccessorTests
    {
        [Fact]
        public void CreatePropertyValueAccessor()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Boolean");

            // Act
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);

            // Assert
            Assert.Equal(valueAccessor.Name, "Boolean");
            Assert.Equal(valueAccessor.Type, propertyInfo.PropertyType);
        }

        [Fact]
        public void ExecutePropertyValueAccessorWithInvalidType()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Single");

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () =>
                    {
                        new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
                    });
        }

        [Fact]
        public void ExecutePropertyValueAccessorWithNullParameter()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new PropertyValueAccessor<EntityWithProperties>(null);
                });
        }

        #region Boolean

        [Fact]
        public void PropertyValueAccessorSetBooleanTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Boolean");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties {Boolean = true};
            const bool newValue = false;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Boolean, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetBooleanTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Boolean");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            const bool value = true;
            var user = new EntityWithProperties { Boolean = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.BooleanValue, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableBooleanTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableBoolean");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableBoolean = true };
            const bool newValue = false;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableBoolean, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableBooleanTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableBoolean");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            const bool value = true;
            var user = new EntityWithProperties { NullableBoolean = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.BooleanValue, value);
        }

        #endregion

        #region Binary

        [Fact]
        public void PropertyValueAccessorSetByteTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Binary");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { Binary = new byte[]{0x11, 0x22, 0x33} };
            var newValue = new byte[] {0x33, 0x22, 0x11};

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Binary, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetBinaryTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Binary");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var value = new byte[] {0x11, 0x22, 0x33};
            var user = new EntityWithProperties { Binary = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.BinaryValue, value);
        }

        #endregion

        #region DateTime

        [Fact]
        public void PropertyValueAccessorSetDateTimeTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("DateTime");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { DateTime = DateTime.MinValue };
            var newValue = DateTime.Now;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.DateTime, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetDateTimeTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("DateTime");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            DateTime value = DateTime.UtcNow;
            var user = new EntityWithProperties { DateTime = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.NotNull(entityProperty.DateTimeOffsetValue);
            Assert.True(entityProperty.DateTimeOffsetValue.HasValue);
            Assert.Equal(entityProperty.DateTimeOffsetValue.Value.DateTime, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableDateTimeTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableDateTime");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableDateTime = null };
            DateTime? newValue = DateTime.UtcNow;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableDateTime, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableDateTimeTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableDateTime");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            DateTime? value = DateTime.UtcNow;
            var user = new EntityWithProperties { NullableDateTime = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.NotNull(entityProperty.DateTimeOffsetValue);
            Assert.True(entityProperty.DateTimeOffsetValue.HasValue);
            Assert.Equal(entityProperty.DateTimeOffsetValue.Value.DateTime, value);
        }

        #endregion

        #region DateTimeOffset

        [Fact]
        public void PropertyValueAccessorSetDateTimeOffsetTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("DateTimeOffset");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { DateTimeOffset = DateTime.Today };
            DateTimeOffset newValue = DateTime.Now;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.DateTimeOffset, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetDateTimeOffsetTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("DateTimeOffset");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            DateTimeOffset value = DateTime.UtcNow;
            var user = new EntityWithProperties { DateTimeOffset = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.DateTimeOffsetValue, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableDateTimeOffsetTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableDateTimeOffset");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableDateTimeOffset = null };
            DateTimeOffset? newValue = DateTime.UtcNow;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableDateTimeOffset, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableDateTimeOffsetTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableDateTimeOffset");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            DateTimeOffset? value = DateTime.UtcNow;
            var user = new EntityWithProperties { NullableDateTimeOffset = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.DateTimeOffsetValue, value);
        }

        #endregion

        #region Double

        [Fact]
        public void PropertyValueAccessorSetDoubleTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Double");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { Double = 0.3 };
            const Double newValue = 0.5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Double, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetDoubleTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Double");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            const Double value = 0.3;
            var user = new EntityWithProperties { Double = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.DoubleValue, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableDoubleTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableDouble");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableDouble = 0.3 };
            Double? newValue = 0.5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableDouble, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableDoubleTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableDouble");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            Double? value = 0.3;
            var user = new EntityWithProperties { NullableDouble = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.DoubleValue, value);
        }

        #endregion

        #region Guid

        [Fact]
        public void PropertyValueAccessorSetGuidTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Guid");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { Guid = new Guid() };
            Guid newValue = Guid.NewGuid();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Guid, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetGuidTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Guid");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            Guid value = Guid.NewGuid();
            var user = new EntityWithProperties { Guid = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.GuidValue, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableGuidTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableGuid");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableGuid = null };
            Guid? newValue = Guid.NewGuid();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableGuid, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableGuidTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableGuid");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            Guid? value = Guid.NewGuid();
            var user = new EntityWithProperties { NullableGuid = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.GuidValue, value);
        }

        #endregion

        #region Int32

        [Fact]
        public void PropertyValueAccessorSetInt32Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Int32");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { Int32 = 2 };
            const Int32 newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Int32, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetInt32Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Int32");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            const Int32 value = 3;
            var user = new EntityWithProperties { Int32 = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.Int32Value, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableInt32Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableInt32");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableInt32 = null };
            Int32? newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableInt32, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableInt32Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableInt32");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            Int32? value = 3;
            var user = new EntityWithProperties { NullableInt32 = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.Int32Value, value);
        }

        #endregion

        #region Int64

        [Fact]
        public void PropertyValueAccessorSetInt64Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Int64");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { Int64 = 2 };
            const Int64 newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Int64, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetInt64Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("Int64");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            const Int64 value = 3;
            var user = new EntityWithProperties { Int64 = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.Int64Value, value);
        }

        [Fact]
        public void PropertyValueAccessorSetNullableInt64Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableInt64");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { NullableInt64 = null };
            Int64? newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableInt64, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetNullableInt64Test()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("NullableInt64");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            Int64? value = 3;
            var user = new EntityWithProperties { NullableInt64 = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.Int64Value, value);
        }

        #endregion

        #region String

        [Fact]
        public void PropertyValueAccessorSetStringTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("String");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            var entity = new EntityWithProperties { String = "aabbcc" };
            const string newValue = "ccbbaa";

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.String, newValue);
        }

        [Fact]
        public void PropertyValueAccessorGetStringTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithProperties).GetProperty("String");
            var valueAccessor = new PropertyValueAccessor<EntityWithProperties>(propertyInfo);
            const string value = "aabbcc";
            var user = new EntityWithProperties { String = value };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityProperty.StringValue, value);
        }

        #endregion
    }
}