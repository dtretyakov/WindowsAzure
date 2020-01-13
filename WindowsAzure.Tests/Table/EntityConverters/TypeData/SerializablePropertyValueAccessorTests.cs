using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public class SerializablePropertyValueAccessorTests
    {
        [Fact]
        public void PropertyValueAccessorSetComplexTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            var valueAccessor = new SerializablePropertyValueAccessor<EntityWithSerializableProperty>(propertyInfo);
            var entity = new EntityWithSerializableProperty {  };

            // Act
            valueAccessor.SetValue(entity, new EntityProperty(JsonConvert.SerializeObject(new SerializableEntity() { IntValue = 5 })));

            // Assert
            Assert.Equal(5, entity.SerializableEntity.IntValue);
        }

        [Fact]
        public void PropertyValueAccessorGetComplexTest()
        {
            // Arrange
            PropertyInfo propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            var valueAccessor = new SerializablePropertyValueAccessor<EntityWithSerializableProperty>(propertyInfo);
            var user = new EntityWithSerializableProperty { SerializableEntity = new SerializableEntity() { IntValue = 2 } };

            // Act
            EntityProperty entityProperty = valueAccessor.GetValue(user);

            // Assert
            Assert.NotNull(entityProperty.StringValue);
        }
    }
}
