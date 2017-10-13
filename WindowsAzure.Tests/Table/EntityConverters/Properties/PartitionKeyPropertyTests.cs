using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.Properties
{
    public sealed class PartitionKeyPropertyTests
    {
        [Fact]
        public void CreatePartitionKey()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");

            // Act
            var result = new PartitionKeyProperty<EntityWithFields>(fieldInfo);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetPartitionKeyValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var property = new PartitionKeyProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity {PartitionKey = "Key"};
            var entity = new EntityWithFields();

            // Act
            property.SetMemberValue(tableEntity, entity);

            // Assert
            Assert.Equal(tableEntity.PartitionKey, entity.String);
        }

        [Fact]
        public void GetPartitionKeyValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var property = new PartitionKeyProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity();
            var entity = new EntityWithFields{ String = "Key"};

            // Act
            property.GetMemberValue(entity, tableEntity);

            // Assert
            Assert.Equal(entity.String, tableEntity.PartitionKey);
        }

        [Fact]
        public void CreatePartitionKeyPropertyWithInvalidType()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Boolean");

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => new PartitionKeyProperty<EntityWithFields>(fieldInfo));
        }

        [Fact]
        public void CreatePartitionKeyPropertyWithNullArgument()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new PartitionKeyProperty<EntityWithFields>(null));
        }
    }
}