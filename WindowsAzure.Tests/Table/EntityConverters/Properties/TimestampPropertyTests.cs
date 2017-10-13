using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.Properties
{
    public sealed class TimestampPropertyTests
    {
        [Fact]
        public void CreateTimestamp()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTime");

            // Act
            var result = new TimestampProperty<EntityWithFields>(fieldInfo);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetTimestampValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTime");
            var property = new TimestampProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity {Timestamp = new DateTime(1980, 1, 1)};
            var entity = new EntityWithFields();

            // Act
            property.SetMemberValue(tableEntity, entity);

            // Assert
            Assert.Equal(tableEntity.Timestamp.UtcDateTime, entity.DateTime);
        }

        [Fact]
        public void GetTimestampValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("DateTime");
            var property = new TimestampProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity();
            var entity = new EntityWithFields{ DateTime = new DateTime(1980, 1, 1)};

            // Act
            property.GetMemberValue(entity, tableEntity);

            // Assert
            Assert.Equal(DateTime.MinValue, tableEntity.Timestamp.UtcDateTime);
        }

        [Fact]
        public void CreateTimestampPropertyWithInvalidType()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Boolean");

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => new TimestampProperty<EntityWithFields>(fieldInfo));
        }

        [Fact]
        public void CreateTimestampPropertyWithNullArgument()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new TimestampProperty<EntityWithFields>(null));
        }
    }
}