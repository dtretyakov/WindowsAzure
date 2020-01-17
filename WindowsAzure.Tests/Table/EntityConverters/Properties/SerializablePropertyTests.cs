using Microsoft.WindowsAzure.Storage.Table;
using System;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.Properties
{
    public sealed class SerializablePropertyTests : SerializerTestBase
    {
        private SerializableProperty<EntityWithSerializableProperty> _objectUnderTest;            

        [Fact]
        public void CreateSerializableProperty()
        {
            // Arrange
            var propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));

            // Act
            _objectUnderTest = new SerializableProperty<EntityWithSerializableProperty>(propertyInfo);

            // Assert
            Assert.NotNull(_objectUnderTest);
        }

        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void SetSerializablePropertyValue(ISerializer serializer)
        {
            // Arrange
            var propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            _objectUnderTest = new SerializableProperty<EntityWithSerializableProperty>(propertyInfo, serializer);
            var tableEntity = new DynamicTableEntity();

            var nestedEntity = new SerializableEntity
            {
                DecimalValue = 26,
            };            

            tableEntity.Properties.Add(
                nameof(EntityWithSerializableProperty.SerializableEntity), new EntityProperty(serializer.Serialize(nestedEntity)));

            var entity = new EntityWithSerializableProperty
            {
                SerializableEntity = nestedEntity,
            };

            // Act
            _objectUnderTest.SetMemberValue(tableEntity, entity);

            // Assert
            var entityProperty = tableEntity.Properties[nameof(EntityWithSerializableProperty.SerializableEntity)].StringValue;
            Assert.Equal(entity.SerializableEntity.DecimalValue, serializer.Deserialize<SerializableEntity>(entityProperty).DecimalValue);
        }


        [Theory]
        [MemberData(nameof(SerializersMemberData))]
        public void GetSerializablePropertyValue(ISerializer serializer)
        {
            // Arrange
            var propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            _objectUnderTest = new SerializableProperty<EntityWithSerializableProperty>(propertyInfo, serializer);
            var tableEntity = new DynamicTableEntity();

            var nestedEntity = new SerializableEntity
            {
                DecimalValue = 26,
            };

            var entity = new EntityWithSerializableProperty 
            {
                SerializableEntity = nestedEntity,
            };

            // Act
            _objectUnderTest.GetMemberValue(entity, tableEntity);

            // Assert
            var entityProperty = tableEntity.Properties[nameof(EntityWithSerializableProperty.SerializableEntity)].StringValue;
            Assert.Equal(entityProperty, serializer.Serialize(nestedEntity));
        }

        [Fact]
        public void CreateSerializablePropertyWithNullMemberArgument()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new SerializableProperty<EntityWithSerializableProperty>(null, new NewtonsoftJsonSerializer()));
        }

        [Fact]
        public void CreateSerializablePropertyWithNullSerializerArgument()
        {
            //Arrange 
            var propertyInfo = typeof(EntityWithSerializableProperty).GetProperty(nameof(EntityWithSerializableProperty.SerializableEntity));
            ISerializer serializer = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => new SerializableProperty<EntityWithSerializableProperty>(propertyInfo, serializer));
        }
    }
}
