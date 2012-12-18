using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class AddEntitiesTests : TableSetTestBase
    {
        [Fact]
        public void AddEntitiesSyncTest()
        {
            // Arrange
            var countries = new List<Country>
                                {
                                    new Country
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
                                        },
                                    new Country
                                        {
                                            Area = 5059922,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1813, 1, 1),
                                            Id = Guid.NewGuid(),
                                            IsExists = false,
                                            Name = "Spain2",
                                            Population = 471904932,
                                            PresidentsCount = 82
                                        }
                                };

            TableSet<Country> tableSet = GetTableSet();

            // Act
            IReadOnlyList<Country> result = tableSet.Add(countries);

            // Assert
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
        public async Task AddEntitiesAsyncTest()
        {
            // Arrange
            var countries = new List<Country>
                                {
                                    new Country
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
                                        },
                                    new Country
                                        {
                                            Area = 3570212,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1872, 1, 18),
                                            Id = Guid.NewGuid(),
                                            IsExists = false,
                                            Name = "Germany2",
                                            Population = 817996002,
                                            PresidentsCount = 112
                                        }
                                };

            TableSet<Country> tableSet = GetTableSet();

            // Act
            IReadOnlyList<Country> result = await tableSet.AddAsync(countries);

            // Assert
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