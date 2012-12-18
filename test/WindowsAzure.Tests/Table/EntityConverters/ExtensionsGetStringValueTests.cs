using WindowsAzure.Table.EntityConverters.Infrastructure;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class ExtensionsGetStringValueTests
    {
        [Fact]
        public void GetStringValueTest()
        {
            // Arrange
            var country = new Country {Continent = "Asia"};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            string value = entityTypeData.PartitionKey.GetStringValue(country);

            // Assert
            Assert.NotNull(value);
            Assert.Equal(value, country.Continent);
        }
    }
}