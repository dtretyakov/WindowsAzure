using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class EntityTypeClassMapTests
    {
        [Fact]
        public void CreateMapper()
        {
            // Arrange & Act
            var map = EntityTypeMap.RegisterClassMap<CountryWithOutAttributes>(e => e.MapPartitionKey(p => p.Continent)
                                                                                          .MapRowKey(p => p.Name));


            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.Equal(2, map.NameChanges.Count);
            Assert.Equal("PartitionKey", map.NameChanges["Continent"]);
            Assert.Equal("RowKey", map.NameChanges["Name"]);
        }
    }
}