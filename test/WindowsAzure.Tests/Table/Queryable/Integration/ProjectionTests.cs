using System;
using System.Linq;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Tests.Attributes;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Integration
{
    internal class ProjectionTests : CountryTableSetBase
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private const string Finland = "Finland";
        private const string France = "France";

        public ProjectionTests()
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
            tableSet.Add(
                new Country
                    {
                        Area = 674843,
                        Continent = "Europe",
                        TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                        Formed = new DateTime(1792, 1, 1),
                        Id = Guid.NewGuid(),
                        IsExists = true,
                        Name = France,
                        Population = 65350000,
                        PresidentsCount = 24
                    });
            tableSet.Add(
                new Country
                    {
                        Area = 338424,
                        Continent = "Europe",
                        TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                        Formed = new DateTime(1809, 3, 29),
                        Id = Guid.NewGuid(),
                        IsExists = true,
                        Name = Finland,
                        Population = 5421827,
                        PresidentsCount = 12
                    });
        }

        [IntegrationFact]
        public void ProjectionWithSync()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var query = from country in tableSet
                        where country.Area > 400000
                        select new {country.Continent, country.Name};

            var values = query.ToList();

            // Assert
            Assert.Equal(2, values.Count);
            Assert.Contains(France, values.Select(p => p.Name));
            Assert.Contains(Spain, values.Select(p => p.Name));
        }

        [IntegrationFact]
        public async Task ProjectionWithAsync()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var query = from country in tableSet
                        where country.Area > 400000
                        select new {country.Continent, country.Name};

            var values = await query.ToListAsync();

            // Assert
            Assert.Equal(2, values.Count);
            Assert.Contains(France, values.Select(p => p.Name));
            Assert.Contains(Spain, values.Select(p => p.Name));
        }
    }
}