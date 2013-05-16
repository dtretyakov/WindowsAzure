using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.Properties
{
    public sealed class RegularPropertyTests
    {
        [Fact]
        public void CreateRegularProperty()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Guid");

            // Act
            var result = new RegularProperty<EntityWithFields>(fieldInfo);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void SetRegularPropertyValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Guid");
            var property = new RegularProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity();
            tableEntity.Properties.Add("Guid", new EntityProperty(Guid.NewGuid()));
            var entity = new EntityWithFields();

            // Act
            property.SetMemberValue(tableEntity, entity);

            // Assert
            Assert.Equal(tableEntity.Properties["Guid"].GuidValue, entity.Guid);
        }

        [Fact]
        public void GetRegularPropertyValue()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Int64");
            var property = new RegularProperty<EntityWithFields>(fieldInfo);
            var tableEntity = new DynamicTableEntity();
            var entity = new EntityWithFields {Int64 = 22};

            // Act
            property.GetMemberValue(entity, tableEntity);

            // Assert
            Assert.Contains("Int64", tableEntity.Properties.Keys);
            Assert.Equal(EdmType.Int64, tableEntity.Properties["Int64"].PropertyType);
            Assert.Equal(entity.Int64, tableEntity.Properties["Int64"].Int64Value);
        }

        [Fact]
        public void CreateRegularPropertyWithInvalidType()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Single");

            // Act
            Assert.Throws<ArgumentException>(() => new RegularProperty<EntityWithFields>(fieldInfo));
        }

        [Fact]
        public void CreateRegularPropertyWithNullMember()
        {
            // Act
            Assert.Throws<ArgumentNullException>(() => new RegularProperty<EntityWithFields>(null, "name"));
        }

        [Fact]
        public void CreateRegularPropertyWithNullName()
        {
            // Arrange
            FieldInfo fieldInfo = typeof (EntityWithFields).GetField("Boolean");

            // Act
            Assert.Throws<ArgumentNullException>(() => new RegularProperty<EntityWithFields>(fieldInfo, null));
        }
    }
}