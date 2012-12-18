using System.Collections.Generic;
using System.Linq;
using WindowsAzure.Table.EntityConverters.Infrastructure;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class EntityTypeDataTests
    {
        [Fact]
        public void CreateEntityTypeDataTest()
        {
            // Arrange & Act
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Assert
            Assert.NotNull(entityTypeData);

            // Check PropertyInfo
            Assert.NotNull(entityTypeData.PartitionKey);
            Assert.Equal(entityTypeData.PartitionKey.Name, "Continent");

            Assert.NotNull(entityTypeData.RowKey);
            Assert.Equal(entityTypeData.RowKey.Name, "Name");

            Assert.NotNull(entityTypeData.Properties);
            List<string> propertyInfoNames = entityTypeData.Properties.Select(p => p.Name).ToList();

            Assert.Contains("Population", propertyInfoNames);
            Assert.Contains("Area", propertyInfoNames);
            Assert.Contains("Formed", propertyInfoNames);
            Assert.Contains("Id", propertyInfoNames);
            Assert.Contains("PresidentsCount", propertyInfoNames);
            Assert.Contains("TopSecretKey", propertyInfoNames);
            Assert.Contains("IsExists", propertyInfoNames);

            // Check name changes
            Assert.NotNull(entityTypeData.NameChanges);

            Assert.True(entityTypeData.NameChanges.ContainsKey("Continent"));
            Assert.Equal(entityTypeData.NameChanges["Continent"], "PartitionKey");
            Assert.True(entityTypeData.NameChanges.ContainsKey("Continent"));
            Assert.Equal(entityTypeData.NameChanges["Name"], "RowKey");
        }
    }
}