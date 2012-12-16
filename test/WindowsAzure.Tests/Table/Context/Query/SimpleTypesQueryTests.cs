using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table;
using GitHub.WindowsAzure.Table.Extensions;
using GitHub.WindowsAzure.Tests.Samples;
using Xunit;

namespace GitHub.WindowsAzure.Tests.Table.Context.Query
{
    public sealed class SimpleTypesQueryTests : TableSetTestBase
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";

        public SimpleTypesQueryTests()
        {
            TableSet<Country> tableSet = GetTableSet();
            tableSet.Add(
                new Country
                    {
                        Area = 357021,
                        Continent = "Europe",
                        TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                        Formed = new DateTime(1871, 1, 18),
                        Id = Guid.NewGuid(),
                        IsExists = true,
                        Name = Germany,
                        Population = 81799600,
                        PresidentsCount = 11
                    });
            tableSet.Add(
                new Country
                    {
                        Area = 505992,
                        Continent = "Europe",
                        TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                        Formed = new DateTime(1812, 1, 1),
                        Id = Guid.NewGuid(),
                        IsExists = false,
                        Name = Spain,
                        Population = 47190493,
                        PresidentsCount = 8
                    });
        }

        [Fact]
        public async Task BinaryQueryWithEqualBooleanValueTest()
        {
            // Arrange
            const bool value = false;
            TableSet<Country> tableSet = GetTableSet();

            // Act
            IQueryable<Country> query = tableSet.Where(p => p.IsExists == value);
            List<Country> values = await query.ToListAsync();

            // Assert
            Assert.Equal(values.Count, 1);
            Assert.Equal(values[0].Name, Spain);
        }

        [Fact]
        public async Task BinaryQueryWithNotEqualBooleanValueTest()
        {
            // Arrange
            const bool value = false;
            TableSet<Country> tableSet = GetTableSet();

            // Act
            IQueryable<Country> query = tableSet.Where(p => p.IsExists != value);

            List<Country> values = await query.ToListAsync();

            // Assert
            Assert.Equal(values.Count, 1);
            Assert.Equal(values[0].Name, Germany);
        }

        [Fact]
        public async Task UnaryQueryWithBooleanValueTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            IQueryable<Country> query = tableSet.Where(p => p.IsExists);

            List<Country> values = await query.ToListAsync();

            // Assert
            Assert.Equal(values.Count, 1);
            Assert.Equal(values[0].Name, Germany);
        }

        [Fact]
        public async Task UnaryQueryWithInversedBooleanValueTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            IQueryable<Country> query = tableSet.Where(p => !p.IsExists);

            List<Country> values = await query.ToListAsync();

            // Assert
            Assert.Equal(values.Count, 1);
            Assert.Equal(values[0].Name, Spain);
        }
    }
}