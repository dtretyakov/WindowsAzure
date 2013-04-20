using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable
{
    public sealed class ComplexQueryTests : CountryTableSetBase
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private const string Finland = "Finland";
        private const string France = "France";

        public ComplexQueryTests()
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

        [Fact]
        public async Task QueryWithMultipleScopesTest()
        {
            // Arrange
            const int value = 1800;
            TableSet<Country> tableSet = GetTableSet();

            // Act
            IQueryable<Country> query =
                tableSet.Where(
                    p => p.Formed > new DateTime(value, 1, 1) &&
                         (p.PresidentsCount < 10 ||
                          p.Population < 10000000 && p.PresidentsCount > 10 && p.IsExists));

            List<Country> values = await query.ToListAsync();

            // Assert
            Assert.Equal(values.Count, 2);
            Assert.Contains(Finland, values.Select(p => p.Name));
            Assert.Contains(Spain, values.Select(p => p.Name));
        }

        [Fact]
        public async Task QueryTableSetTwiceTest()
        {
            // Arrange
            const int value = 1800;
            TableSet<Country> tableSet = GetTableSet();

            // Act
            IQueryable<Country> queryTwoEntity =
                tableSet.Where(
                    p => p.Formed > new DateTime(value, 1, 1) &&
                         (p.PresidentsCount < 10 ||
                          p.Population < 10000000 && p.PresidentsCount > 10 && p.IsExists));

            var queryOneEntity = tableSet.Where(p => p.Name == Germany);

            var result = await queryOneEntity.SingleAsync();
            List<Country> results = await queryTwoEntity.ToListAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(result.Name, Germany);
            Assert.Equal(results.Count, 2);
            Assert.Contains(Finland, results.Select(p => p.Name));
            Assert.Contains(Spain, results.Select(p => p.Name));
        }

        [Fact]
        public void QueryWithSelectClause()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var query = from country in tableSet
                        where country.Area > 400000
                        select new {country.Continent, country.Name};

            var values = query.ToList();

            // Assert
            Assert.Equal(values.Count, 2);
            Assert.Contains(France, values.Select(p => p.Name));
            Assert.Contains(Spain, values.Select(p => p.Name));
        }

        [Fact]
        public async Task QueryWithSelectClauseAsync()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var query = from country in tableSet
                        where country.Area > 400000
                        select new { country.Continent, country.Name };

            var values = await query.ToListAsync();

            // Assert
            Assert.Equal(values.Count, 2);
            Assert.Contains(France, values.Select(p => p.Name));
            Assert.Contains(Spain, values.Select(p => p.Name));
        }

        [Fact]
        public void QueryWithFirstClause()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var result = tableSet.First(p => p.Name == Finland);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Finland, result.Name);
        }

        [Fact]
        public void QueryWithFirstClauseWithoutResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country result = null;

            // Act
            Assert.Throws<InvalidOperationException>(() =>
                {
                    result = tableSet.First(p => p.Name == string.Empty);
                });
            Assert.Null(result);
        }

        [Fact]
        public void QueryWithFirstOrDefaultClause()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var result = tableSet.FirstOrDefault(p => p.Name == Finland);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Finland, result.Name);
        }

        [Fact]
        public void QueryWithFirstOrDefaultClauseWithoutResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var result = tableSet.FirstOrDefault(p => p.Name == string.Empty);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void QueryWithSingleClause()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var result = tableSet.Single(p => p.Name == Finland);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Finland, result.Name);
        }

        [Fact]
        public void QueryWithSingleClauseWithoutResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country result = null;

            // Act
            Assert.Throws<InvalidOperationException>(() =>
            {
                result = tableSet.Single(p => p.Name == string.Empty);
            });
            Assert.Null(result);
        }

        [Fact]
        public void QueryWithSingleOrDefaultClause()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var result = tableSet.SingleOrDefault(p => p.Name == Finland);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Finland, result.Name);
        }

        [Fact]
        public void QueryWithSingleOrDefaultClauseWithoutResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            var result = tableSet.SingleOrDefault(p => p.Name == string.Empty);

            // Assert
            Assert.Null(result);
        }
    }
}