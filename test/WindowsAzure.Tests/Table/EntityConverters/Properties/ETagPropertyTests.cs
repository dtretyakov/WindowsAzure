using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.Properties
{
    public sealed class ETagPropertyTests
    {
        [Fact]
        public void CreateETag()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");

            // Act
            var result = new ETagProperty<EntityWithFields>(fieldInfo);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetETagValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var property = new ETagProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity {ETag = "*"};
            var entity = new EntityWithFields();

            // Act
            property.SetMemberValue(tableEntity, entity);

            // Assert
            Assert.Equal(tableEntity.ETag, entity.String);
        }

        [Fact]
        public void GetETagValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof(EntityWithFields).GetField("String");
            var property = new ETagProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity();
            var entity = new EntityWithFields{ String = "*"};

            // Act
            property.GetMemberValue(entity, tableEntity);

            // Assert
            Assert.Equal(entity.String, tableEntity.ETag);
        }

        [Fact]
        public void CreateETagPropertyWithInvalidType()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Boolean");

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => new ETagProperty<EntityWithFields>(fieldInfo));
        }

        [Fact]
        public void CreateETagPropertyWithNullArgument()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new ETagProperty<EntityWithFields>(null));
        }
    }
}