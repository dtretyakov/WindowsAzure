using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.Properties
{
    public sealed class RowKeyPropertyTests
    {
        [Fact]
        public void CreateRowKey()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");

            // Act
            var result = new RowKeyProperty<EntityWithFields>(fieldInfo);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetRowKeyValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var property = new RowKeyProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity {RowKey = "Key"};
            var entity = new EntityWithFields();

            // Act
            property.SetMemberValue(tableEntity, entity);

            // Assert
            Assert.Equal(tableEntity.RowKey, entity.String);
        }

        [Fact]
        public void GetRowKeyValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var property = new RowKeyProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity();
            var entity = new EntityWithFields{ String = "Key"};

            // Act
            property.GetMemberValue(entity, tableEntity);

            // Assert
            Assert.Equal(entity.String, tableEntity.RowKey);
        }

        [Fact]
        public void CreateRowKeyPropertyWithInvalidType()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Boolean");

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => new RowKeyProperty<EntityWithFields>(fieldInfo));
        }

        [Fact]
        public void CreateRowKeyPropertyWithNullArgument()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new RowKeyProperty<EntityWithFields>(null));
        }
    }
}