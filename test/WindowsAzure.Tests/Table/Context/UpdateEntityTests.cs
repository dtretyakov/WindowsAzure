using System;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class UpdateEntityTests : TableSetTestBase
    {
        [Fact]
        public void UpdateEntitySyncTest()
        {
            //Arrange
            var country = new Country
                              {
                                  Area = 17075400,
                                  Continent = "Transcontinental",
                                  TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                  Formed = new DateTime(1721, 10, 22),
                                  Id = Guid.NewGuid(),
                                  IsExists = true,
                                  Name = "Russia",
                                  Population = 143300000,
                                  PresidentsCount = 4
                              };

            TableSet<Country> tableSet = GetTableSet();

            tableSet.Add(country);

            // Act
            country.Population += 333333;
            country.IsExists = false;
            country.PresidentsCount += 5;
            country.TopSecretKey = new byte[] {0xff, 0xee, 0xdd};

            Country result = tableSet.Update(country);

            //Assert
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
        public async Task UpdateEntityAsyncTest()
        {
            //Arrange
            var country = new Country
                              {
                                  Area = 243610,
                                  Continent = "Europe",
                                  TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                  Formed = new DateTime(1801, 1, 1),
                                  Id = Guid.NewGuid(),
                                  IsExists = true,
                                  Name = "United Kingdom",
                                  Population = 62262000,
                                  PresidentsCount = 0
                              };

            TableSet<Country> tableSet = GetTableSet();

            await tableSet.AddAsync(country);

            // Act
            country.Population += 333333;
            country.IsExists = false;
            country.PresidentsCount += 5;
            country.TopSecretKey = new byte[] {0xff, 0xee, 0xdd};

            Country result = await tableSet.UpdateAsync(country);

            //Assert
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