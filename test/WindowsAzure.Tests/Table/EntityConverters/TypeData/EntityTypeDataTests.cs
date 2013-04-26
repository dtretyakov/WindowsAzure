using System;
using WindowsAzure.Table.EntityConverters.TypeData;
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
        public void CreateEntityTypeDataWithIvalidFields()
        {
            // Arrange
            EntityTypeData<EntityWithFields> typeData = null;

            // Act
            Assert.Throws<ArgumentException>(() => { typeData = new EntityTypeData<EntityWithFields>(); });

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
    }
}