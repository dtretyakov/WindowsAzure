using System;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class AddOrUpdateEntityTests : CountryTableSetBase
    {
        [Fact]
        public void AddOrUpdateEntitySyncTest()
        {
            // Arrange
            var country = new Country
                              {
                                  Area = 505992,
                                  Continent = "Europe",
                                  TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                  Formed = new DateTime(1812, 1, 1),
                                  Id = Guid.NewGuid(),
                                  IsExists = true,
                                  Name = "Spain",
                                  Population = 47190493,
                                  PresidentsCount = 8
                              };

            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country result = tableSet.Add(country);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(country.Area, result.Area);
            Assert.Equal(country.Continent, result.Continent);
            Assert.Equal(country.TopSecretKey, result.TopSecretKey);
            Assert.Equal(country.Formed, result.Formed);
            Assert.Equal(country.Id, result.Id);
            Assert.Equal(country.IsExists, result.IsExists);
            Assert.Equal(country.Name, result.Name);
            Assert.Equal(country.Population, result.Population);
            Assert.Equal(country.PresidentsCount, result.PresidentsCount);
        }

        [Fact]
        public async Task AddOrUpdateEntityAsyncTest()
        {
            // Arrange
            var country = new Country
                              {
                                  Area = 357021,
                                  Continent = "Europe",
                                  TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                  Formed = new DateTime(1871, 1, 18),
                                  Id = Guid.NewGuid(),
                                  IsExists = true,
                                  Name = "Germany",
                                  Population = 81799600,
                                  PresidentsCount = 11
                              };

            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country result = await tableSet.AddAsync(country);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(country.Area, result.Area);
            Assert.Equal(country.Continent, result.Continent);
            Assert.Equal(country.TopSecretKey, result.TopSecretKey);
            Assert.Equal(country.Formed, result.Formed);
            Assert.Equal(country.Id, result.Id);
            Assert.Equal(country.IsExists, result.IsExists);
            Assert.Equal(country.Name, result.Name);
            Assert.Equal(country.Population, result.Population);
            Assert.Equal(country.PresidentsCount, result.PresidentsCount);
        }
    }
}