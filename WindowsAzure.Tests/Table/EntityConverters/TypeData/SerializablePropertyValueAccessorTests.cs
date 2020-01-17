using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public class SerializablePropertyValueAccessorTests : SerializerTestBase
    { 
        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void PropertyValueAccessorSetComplexTest(ISerializer serializer)
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            var valueAccessor = new SerializablePropertyValueAccessor<EntityWithSerializableProperty>(propertyInfo, serializer);
            var entity = new EntityWithSerializableProperty();

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(serializer.Serialize(new SerializableEntity() { DecimalValue = 5 })));

            // Assert
            Assert.Equal(5, entity.SerializableEntity.DecimalValue);
        }

        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void PropertyValueAccessorGetComplexTest(ISerializer serializer)
        {
            //Arrange
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            var valueAccessor = new SerializablePropertyValueAccessor<EntityWithSerializableProperty>(propertyInfo, serializer);
            var user = new EntityWithSerializableProperty { SerializableEntity = new SerializableEntity() { DecimalValue = 2 } };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.NotNull(entityProperty.StringValue);
            Assert.Equal(user.SerializableEntity.DecimalValue, serializer.Deserialize<SerializableEntity>(entityProperty.StringValue).DecimalValue);
        }
    }
}
