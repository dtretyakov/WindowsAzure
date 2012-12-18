using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class DeleteEntitiesTests : TableSetTestBase
    {
        [Fact]
        public void DeleteEntitiesSyncTest()
        {
            //Arrange
            var countries = new List<Country>
                                {
                                    new Country
                                        {
                                            Area = 674843,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1792, 1, 1),
                                            Id = Guid.NewGuid(),
                                            IsExists = true,
                                            Name = "France",
                                            Population = 65350000,
                                            PresidentsCount = 24
                                        },
                                    new Country
                                        {
                                            Area = 6748432,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1793, 1, 1),
                                            Id = Guid.NewGuid(),
                                            IsExists = false,
                                            Name = "France2",
                                            Population = 653500002,
                                            PresidentsCount = 242
                                        }
                                };

            TableSet<Country> tableSet = GetTableSet();

            tableSet.Add(countries);

            // Act
            tableSet.Remove(countries);
        }

        [Fact]
        public async Task DeleteEntitiesAsyncTest()
        {
            //Arrange
            var countries = new List<Country>
                                {
                                    new Country
                                        {
                                            Area = 338424,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1809, 3, 29),
                                            Id = Guid.NewGuid(),
                                            IsExists = true,
                                            Name = "Finland",
                                            Population = 5421827,
                                            PresidentsCount = 12
                                        },
                                    new Country
                                        {
                                            Area = 3384242,
                                            Continent = "Europe",
                                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                                            Formed = new DateTime(1810, 3, 29),
                                            Id = Guid.NewGuid(),
                                            IsExists = false,
                                            Name = "Finland2",
                                            Population = 54218272,
                                            PresidentsCount = 122
                                        }
                                };

            TableSet<Country> tableSet = GetTableSet();

            await tableSet.AddAsync(countries);

            // Act
            await tableSet.RemoveAsync(countries);
        }
    }
}