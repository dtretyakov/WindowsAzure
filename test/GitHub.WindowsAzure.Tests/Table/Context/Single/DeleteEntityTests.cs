using System;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table;
using GitHub.WindowsAzure.Tests.Samples;
using Xunit;

namespace GitHub.WindowsAzure.Tests.Table.Context.Single
{
    public sealed class DeleteEntityTests : TableSetTestBase
    {
        [Fact]
        public void DeleteEntitySyncTest()
        {
            //Arrange
            var country = new Country
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
                              };

            TableSet<Country> tableSet = GetTableSet();

            tableSet.Add(country);

            // Act
            tableSet.Remove(country);
        }

        [Fact]
        public async Task DeleteEntityAsyncTest()
        {
            //Arrange
            var country = new Country
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
                              };

            TableSet<Country> tableSet = GetTableSet();

            await tableSet.AddAsync(country);

            // Act
            await tableSet.RemoveAsync(country);
        }
    }
}