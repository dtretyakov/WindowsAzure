using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class EntityTypeDataTests
    {
        [Fact]
        public void CreateEntityTypeData()
        {
            // Arrange & Act
            var entityTypeData = new EntityTypeData<Country>();

            // Assert
            Assert.NotNull(entityTypeData);
            Assert.NotNull(entityTypeData.NameChanges);
            Assert.True(entityTypeData.NameChanges.ContainsKey("Continent"));
            Assert.Equal(entityTypeData.NameChanges["Continent"], "PartitionKey");
            Assert.True(entityTypeData.NameChanges.ContainsKey("Continent"));
            Assert.Equal(entityTypeData.NameChanges["Name"], "RowKey");
        }

        [Fact]
        public void CreateEntityTypeDataWithDifferentAccessors()
        {
            // Arrange & Act
            var entityTypeData = new EntityTypeData<Entity>();

            // Assert
            Assert.NotNull(entityTypeData);
            Assert.NotNull(entityTypeData.NameChanges);
            Assert.True(entityTypeData.NameChanges.ContainsKey("FirstName"));
            Assert.Equal(entityTypeData.NameChanges["FirstName"], "PartitionKey");
            Assert.True(entityTypeData.NameChanges.ContainsKey("LastName"));
            Assert.Equal(entityTypeData.NameChanges["LastName"], "RowKey");
        }

        [Fact]
        public void CreateEntityTypeDataWithIvalidProperties()
        {
            // Arrange
            EntityTypeData<EntityWithProperties> typeData = null;

            // Act
            Assert.Throws<ArgumentException>(() => { typeData = new EntityTypeData<EntityWithProperties>(); });

            // Assert
            Assert.Null(typeData);
        }

        [Fact]
        public void CreateEntityTypeDataWithFactory()
        {
            // Arrange & Act
            IEntityTypeData<Country> entityTypeData = EntityTypeDataFactory.GetEntityTypeData<Country>();

            // Assert
            Assert.NotNull(entityTypeData);

            // Check name changes
            Assert.NotNull(entityTypeData.NameChanges);

            Assert.True(entityTypeData.NameChanges.ContainsKey("Continent"));
            Assert.Equal(entityTypeData.NameChanges["Continent"], "PartitionKey");
            Assert.True(entityTypeData.NameChanges.ContainsKey("Continent"));
            Assert.Equal(entityTypeData.NameChanges["Name"], "RowKey");
        }

        [Fact]
        public void GetPocoEntityFromNull()
        {
            // Arrange
            var entityTypeData = new EntityTypeData<Entity>();
            Entity entity = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { entity = entityTypeData.GetEntity((DynamicTableEntity)null); });

            // Assert
            Assert.Null(entity);
        }

        [Fact]
        public void GetTableEntityFromNull()
        {
            // Arrange
            var entityTypeData = new EntityTypeData<Entity>();
            ITableEntity entity = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { entity = entityTypeData.GetEntity((Entity)null); });

            // Assert
            Assert.Null(entity);
        }

        [Fact]
        public void CreateEntityTypeDataWithoutCompositeKey()
        {
            // Arrange
            EntityTypeData<EntityWithoutCompositeKey> typeData = null;

            // Act
            Assert.Throws<InvalidOperationException>(() => { typeData = new EntityTypeData<EntityWithoutCompositeKey>(); });

            // Assert
            Assert.Null(typeData);
        }

        [Fact]
        public void CreateEntityTypeDataWithMultipleAttributes()
        {
            // Arrange
            EntityTypeData<EntityWithMultipleAttributes> typeData = null;

            // Act
            Assert.Throws<InvalidOperationException>(() => { typeData = new EntityTypeData<EntityWithMultipleAttributes>(); });

            // Assert
            Assert.Null(typeData);
        }

        [Fact]
        public void CreateEntityTypeDataWithSerializableAttribute()
        {
            // Arrange
            var entityTypeData = new EntityTypeData<EntityWithSerializableAttribute>();
            
            // Assert
            Assert.NotNull(entityTypeData);
            Assert.NotNull(entityTypeData.NameChanges);
            Assert.Equal("PartitionKey", entityTypeData.NameChanges["Pk"]);
            Assert.Equal("RowKey", entityTypeData.NameChanges["Rk"]);
            Assert.Equal("NestedSerialized", entityTypeData.NameChanges["Nested"]);
        }
    }
}