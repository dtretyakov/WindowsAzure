using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public class SerializableValueAccessorTests : SerializerTestBase
    {
        [Fact]
        public void ExecutePropertyValueAccessorWithNullParameter()
        {
            //Arrange
            ISerializer serializer = new NewtonsoftJsonSerializer();
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new SerializableValueAccessor<EntityWithSerializableProperty>(null, serializer);
                });

            // Act & Assert
            Assert.Throws<ArgumentNullException>(
                () =>
                {
                    new SerializableValueAccessor<EntityWithSerializableProperty>(propertyInfo, null);
                });
        }


        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void SerializablePropertyValueAccessorSetWithComplexPropertiesTest(ISerializer serializer)
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            var valueAccessor = new SerializableValueAccessor<EntityWithSerializableProperty>(propertyInfo, serializer);
            var entity = new EntityWithSerializableProperty();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(serializer.Serialize(new SerializableEntity() { DecimalValue = 5 })));

            // Assert
            Assert.Equal(5, entity.SerializableEntity.DecimalValue);
        }

        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void SerializablePropertyValueAccessorGetWithComplexPropertiesTest(ISerializer serializer)
        {
            //Arrange
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            var valueAccessor = new SerializableValueAccessor<EntityWithSerializableProperty>(propertyInfo, serializer);
            var entity = new EntityWithSerializableProperty { SerializableEntity = new SerializableEntity() { DecimalValue = 2 } };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(entity);

            // Assert
            Assert.NotNull(entityProperty.StringValue);
            Assert.Equal(entity.SerializableEntity.DecimalValue, serializer.Deserialize<SerializableEntity>(entityProperty.StringValue).DecimalValue);
        }

        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void SerializablePropertyValueAccessorSetWithComplexFieldsTest(ISerializer serializer)
        {
            // Arrange
            var fieldInfo = typeof(EntityWithSerializableField).GetField(nameof(EntityWithSerializableField.DecimalValue));
            var valueAccessor = new SerializableValueAccessor<EntityWithSerializableField>(fieldInfo, serializer);
            var entity = new EntityWithSerializableField();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(serializer.Serialize(5)));

            // Assert
            Assert.Equal(5, entity.DecimalValue);
        }

        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void SerializablePropertyValueAccessorGetWithComplexFieldsTest(ISerializer serializer)
        {
            //Arrange
            var fieldInfo = typeof(EntityWithSerializableField).GetField(nameof(EntityWithSerializableField.DecimalValue));
            var valueAccessor = new SerializableValueAccessor<EntityWithSerializableField>(fieldInfo, serializer);
            var entity = new EntityWithSerializableField { DecimalValue = 5 };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(entity);

            // Assert
            Assert.NotNull(entityProperty.StringValue);
            Assert.Equal(entity.DecimalValue, serializer.Deserialize<decimal>(entityProperty.StringValue));
        }
    }
}
