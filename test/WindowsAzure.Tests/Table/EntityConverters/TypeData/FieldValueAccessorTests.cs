using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class FieldValueAccessorTests
    {
        [Fact]
        public void FieldValueAccessorCreateTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Boolean");

            // Act
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);

            // Assert
            Assert.Equal(valueAccessor.Name, "Boolean");
            Assert.Equal(valueAccessor.Type, fieldInfo.FieldType);
        }

        [Fact]
        public void FieldValueAccessorInvalidTypeTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Single");

            // Act & Assert
            Assert.Throws<ArgumentException>(
                () =>
                    {
                        new FieldValueAccessor<EntityWithFields>(fieldInfo);
                    });
        }

        #region Boolean

        [Fact]
        public void FieldValueAccessorSetBooleanTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Boolean");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields {Boolean = true};
            const bool newValue = false;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Boolean, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetBooleanTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Boolean");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            const bool value = true;
            var user = new EntityWithFields { Boolean = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.BooleanValue, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableBooleanTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableBoolean");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableBoolean = true };
            const bool newValue = false;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableBoolean, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableBooleanTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableBoolean");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            const bool value = true;
            var user = new EntityWithFields { NullableBoolean = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.BooleanValue, value);
        }

        #endregion

        #region Binary

        [Fact]
        public void FieldValueAccessorSetBinaryTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Binary");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { Binary = new byte[]{0x11, 0x22, 0x33} };
            var newValue = new byte[] {0x33, 0x22, 0x11};

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Binary, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetBinaryTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Binary");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var value = new byte[] {0x11, 0x22, 0x33};
            var user = new EntityWithFields { Binary = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.BinaryValue, value);
        }

        #endregion

        #region DateTime

        [Fact]
        public void FieldValueAccessorSetDateTimeTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTime");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { DateTime = DateTime.MinValue };
            var newValue = DateTime.Now;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.DateTime, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetDateTimeTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTime");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            DateTime value = DateTime.UtcNow;
            var user = new EntityWithFields { DateTime = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.NotNull(entityField.DateTimeOffsetValue);
            Assert.True(entityField.DateTimeOffsetValue.HasValue);
            Assert.Equal(entityField.DateTimeOffsetValue.Value.DateTime, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableDateTimeTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableDateTime");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableDateTime = null };
            DateTime? newValue = DateTime.UtcNow;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableDateTime, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableDateTimeTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableDateTime");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            DateTime? value = DateTime.UtcNow;
            var user = new EntityWithFields { NullableDateTime = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.NotNull(entityField.DateTimeOffsetValue);
            Assert.True(entityField.DateTimeOffsetValue.HasValue);
            Assert.Equal(entityField.DateTimeOffsetValue.Value.DateTime, value);
        }

        #endregion

        #region DateTimeOffset

        [Fact]
        public void FieldValueAccessorSetDateTimeOffsetTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTimeOffset");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { DateTimeOffset = DateTime.Today };
            DateTimeOffset newValue = DateTime.Now;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.DateTimeOffset, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetDateTimeOffsetTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTimeOffset");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            DateTimeOffset value = DateTime.UtcNow;
            var user = new EntityWithFields { DateTimeOffset = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.DateTimeOffsetValue, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableDateTimeOffsetTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableDateTimeOffset");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableDateTimeOffset = null };
            DateTimeOffset? newValue = DateTime.UtcNow;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableDateTimeOffset, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableDateTimeOffsetTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableDateTimeOffset");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            DateTimeOffset? value = DateTime.UtcNow;
            var user = new EntityWithFields { NullableDateTimeOffset = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.DateTimeOffsetValue, value);
        }

        #endregion

        #region Double

        [Fact]
        public void FieldValueAccessorSetDoubleTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Double");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { Double = 0.3 };
            const Double newValue = 0.5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Double, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetDoubleTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Double");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            const Double value = 0.3;
            var user = new EntityWithFields { Double = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.DoubleValue, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableDoubleTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableDouble");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableDouble = 0.3 };
            Double? newValue = 0.5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableDouble, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableDoubleTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableDouble");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            Double? value = 0.3;
            var user = new EntityWithFields { NullableDouble = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.DoubleValue, value);
        }

        #endregion

        #region Guid

        [Fact]
        public void FieldValueAccessorSetGuidTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Guid");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { Guid = new Guid() };
            Guid newValue = Guid.NewGuid();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Guid, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetGuidTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Guid");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            Guid value = Guid.NewGuid();
            var user = new EntityWithFields { Guid = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.GuidValue, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableGuidTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableGuid");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableGuid = null };
            Guid? newValue = Guid.NewGuid();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableGuid, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableGuidTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableGuid");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            Guid? value = Guid.NewGuid();
            var user = new EntityWithFields { NullableGuid = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.GuidValue, value);
        }

        #endregion

        #region Int32

        [Fact]
        public void FieldValueAccessorSetInt32Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Int32");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { Int32 = 2 };
            const Int32 newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Int32, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetInt32Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Int32");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            const Int32 value = 3;
            var user = new EntityWithFields { Int32 = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.Int32Value, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableInt32Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableInt32");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableInt32 = null };
            Int32? newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableInt32, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableInt32Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableInt32");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            Int32? value = 3;
            var user = new EntityWithFields { NullableInt32 = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.Int32Value, value);
        }

        #endregion

        #region Int64

        [Fact]
        public void FieldValueAccessorSetInt64Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Int64");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { Int64 = 2 };
            const Int64 newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.Int64, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetInt64Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("Int64");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            const Int64 value = 3;
            var user = new EntityWithFields { Int64 = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.Int64Value, value);
        }

        [Fact]
        public void FieldValueAccessorSetNullableInt64Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableInt64");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { NullableInt64 = null };
            Int64? newValue = 5;

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.NullableInt64, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetNullableInt64Test()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("NullableInt64");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            Int64? value = 3;
            var user = new EntityWithFields { NullableInt64 = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.Int64Value, value);
        }

        #endregion

        #region String

        [Fact]
        public void FieldValueAccessorSetStringTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            var entity = new EntityWithFields { String = "aabbcc" };
            const string newValue = "ccbbaa";

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(newValue));

            // Assert
            Assert.Equal(entity.String, newValue);
        }

        [Fact]
        public void FieldValueAccessorGetStringTest()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var valueAccessor = new FieldValueAccessor<EntityWithFields>(fieldInfo);
            const string value = "aabbcc";
            var user = new EntityWithFields { String = value };

            // Act
            EntityProperty entityField = valueAccessor.GetValue(user);

            // Assert
            Assert.Equal(entityField.StringValue, value);
        }

        #endregion
    }
}