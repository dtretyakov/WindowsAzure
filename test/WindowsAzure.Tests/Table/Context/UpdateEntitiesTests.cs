using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class UpdateEntitiesTests : TableSetTestBase
    {
        [Fact]
        public void UpdateEntitiesSyncTest()
        {
            //Arrange
            var countries = new List<Country>
                                {
                                    new Country
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
                                        },
                                    new Country
                                        {
                                            Area = 170754002,
                                            Continent = "Transcontinental",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1722, 10, 22),
                                            Id = Guid.NewGuid(),
                                            IsExists = false,
                                            Name = "Russia2",
                                            Population = 1433000002,
                                            PresidentsCount = 42
                                        }
                                };

            TableSet<Country> tableSet = GetTableSet();

            tableSet.Add(countries);

            // Act
            countries[0].Population += 333333;
            countries[0].IsExists = false;
            countries[0].PresidentsCount += 5;
            countries[0].TopSecretKey = new byte[] {0xff, 0xee, 0xdd};

            countries[1].Population += 333333;
            countries[1].IsExists = false;
            countries[1].PresidentsCount += 5;
            countries[1].TopSecretKey = new byte[] {0xff, 0xee, 0xdd};

            List<Country> result = tableSet.Update(countries).ToList();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, 2);

            Assert.Equal(countries[0].Area, result[0].Area);
            Assert.Equal(countries[0].Continent, result[0].Continent);
            Assert.Equal(countries[0].TopSecretKey, result[0].TopSecretKey);
            Assert.Equal(countries[0].Formed, result[0].Formed);
            Assert.Equal(countries[0].Id, result[0].Id);
            Assert.Equal(countries[0].IsExists, result[0].IsExists);
            Assert.Equal(countries[0].Name, result[0].Name);
            Assert.Equal(countries[0].Population, result[0].Population);
            Assert.Equal(countries[0].PresidentsCount, result[0].PresidentsCount);

            Assert.Equal(countries[1].Area, result[1].Area);
            Assert.Equal(countries[1].Continent, result[1].Continent);
            Assert.Equal(countries[1].TopSecretKey, result[1].TopSecretKey);
            Assert.Equal(countries[1].Formed, result[1].Formed);
            Assert.Equal(countries[1].Id, result[1].Id);
            Assert.Equal(countries[1].IsExists, result[1].IsExists);
            Assert.Equal(countries[1].Name, result[1].Name);
            Assert.Equal(countries[1].Population, result[1].Population);
            Assert.Equal(countries[1].PresidentsCount, result[1].PresidentsCount);
        }

        [Fact]
        public async Task UpdateEntitiesAsyncTest()
        {
            //Arrange
            var countries = new List<Country>
                                {
                                    new Country
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
                                        },
                                    new Country
                                        {
                                            Area = 2436102,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1802, 1, 1),
                                            Id = Guid.NewGuid(),
                                            IsExists = false,
                                            Name = "United Kingdom2",
                                            Population = 622620002,
                                            PresidentsCount = 2
                                        }
                                };

            TableSet<Country> tableSet = GetTableSet();

            await tableSet.AddAsync(countries);

            // Act
            countries[0].Population += 333333;
            countries[0].IsExists = false;
            countries[0].PresidentsCount += 5;
            countries[0].TopSecretKey = new byte[] {0xff, 0xee, 0xdd};

            countries[1].Population += 333333;
            countries[1].IsExists = false;
            countries[1].PresidentsCount += 5;
            countries[1].TopSecretKey = new byte[] {0xff, 0xee, 0xdd};

            List<Country> result = (await tableSet.UpdateAsync(countries)).ToList();

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result.Count, 2);

            Assert.Equal(countries[0].Area, result[0].Area);
            Assert.Equal(countries[0].Continent, result[0].Continent);
            Assert.Equal(countries[0].TopSecretKey, result[0].TopSecretKey);
            Assert.Equal(countries[0].Formed, result[0].Formed);
            Assert.Equal(countries[0].Id, result[0].Id);
            Assert.Equal(countries[0].IsExists, result[0].IsExists);
            Assert.Equal(countries[0].Name, result[0].Name);
            Assert.Equal(countries[0].Population, result[0].Population);
            Assert.Equal(countries[0].PresidentsCount, result[0].PresidentsCount);

            Assert.Equal(countries[1].Area, result[1].Area);
            Assert.Equal(countries[1].Continent, result[1].Continent);
            Assert.Equal(countries[1].TopSecretKey, result[1].TopSecretKey);
            Assert.Equal(countries[1].Formed, result[1].Formed);
            Assert.Equal(countries[1].Id, result[1].Id);
            Assert.Equal(countries[1].IsExists, result[1].IsExists);
            Assert.Equal(countries[1].Name, result[1].Name);
            Assert.Equal(countries[1].Population, result[1].Population);
            Assert.Equal(countries[1].PresidentsCount, result[1].PresidentsCount);
        }
    }
}