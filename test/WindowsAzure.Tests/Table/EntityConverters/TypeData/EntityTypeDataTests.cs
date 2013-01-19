using System;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class EntityTypeDataTests
    {
        [Fact]
        public void CreateEntityTypeDataTest()
        {
            // Arrange & Act
            var entityTypeData = new EntityTypeData<Country>();

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
        public void CreateEntityTypeDataWithDifferentAccessorsTest()
        {
            // Arrange & Act
            var entityTypeData = new EntityTypeData<User>();

            // Assert
            Assert.NotNull(entityTypeData);
            Assert.NotNull(entityTypeData.NameChanges);
            Assert.True(entityTypeData.NameChanges.ContainsKey("FirstName"));
            Assert.Equal(entityTypeData.NameChanges["FirstName"], "PartitionKey");
            Assert.True(entityTypeData.NameChanges.ContainsKey("LastName"));
            Assert.Equal(entityTypeData.NameChanges["LastName"], "RowKey");
        }

        [Fact]
        public void CreateEntityTypeWithFactoryTest()
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
        public void CreateEntityTypeDataWithIvalidPropertiesTest()
        {
            // Act & Assert
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => { new EntityTypeData<EntityWithProperties>(); });
            // ReSharper restore ObjectCreationAsStatement
        }

        [Fact]
        public void CreateEntityTypeDataWithIvalidFieldsTest()
        {
            // Act & Assert
            // ReSharper disable ObjectCreationAsStatement
            Assert.Throws<ArgumentException>(() => { new EntityTypeData<EntityWithFields>(); });
            // ReSharper restore ObjectCreationAsStatement
        }
    }
}