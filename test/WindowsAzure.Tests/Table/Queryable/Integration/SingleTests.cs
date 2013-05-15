using System;
using System.Linq;
using WindowsAzure.Table;
using WindowsAzure.Tests.Attributes;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Integration
{
    public sealed class SingleTests : CountryTableSetBase
    {
        private const string Finland = "Finland";
        private const string France = "France";
        private const string Yugoslavia = "Yugoslavia";
        private const string Europe = "Europe";

        public SingleTests()
        {
            TableSet<Country> tableSet = GetTableSet();
            tableSet.Add(
                new Country
                    {
                        Area = 674843,
                        Continent = Europe,
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
                        Continent = Europe,
                        TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                        Formed = new DateTime(1809, 3, 29),
                        Id = Guid.NewGuid(),
                        IsExists = true,
                        Name = Finland,
                        Population = 5421827,
                        PresidentsCount = 12
                    });
            tableSet.Add(
                new Country
                    {
                        Area = 2345409,
                        Continent = Europe,
                        TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                        Formed = new DateTime(1929, 1, 1),
                        Id = Guid.NewGuid(),
                        IsExists = false,
                        Name = Yugoslavia,
                        Population = 23229846,
                        PresidentsCount = 1
                    });
        }

        [IntegrationFact]
        public void Single()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country result = null;

            // Act
            Assert.Throws<InvalidOperationException>(() => result = tableSet.Single());

            // Assert
            Assert.Null(result);
        }

        [IntegrationFact]
        public void SingleWithExpressionWithOneResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country result = tableSet.Single(p => p.Name == France);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(France, result.Name);
        }

        [IntegrationFact]
        public void SingleWithExpressionWithMultipleResults()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country result = null;

            // Act
            Assert.Throws<InvalidOperationException>(() => result = tableSet.Single(p => p.Continent == Europe));

            // Assert
            Assert.Null(result);
        }

        [IntegrationFact]
        public void SingleWithoutResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country result = null;

            // Act
            Assert.Throws<InvalidOperationException>(() => { result = tableSet.Single(p => p.Name == string.Empty); });
            Assert.Null(result);
        }

        // ReSharper disable ReplaceWithSingleCallToSingle

        [IntegrationFact]
        public void SingleAfterWhere()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country result = tableSet.Where(p => p.Population > 60000000).Single();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(France, result.Name);
        }

        [IntegrationFact]
        public void SingleAfterWhereWithMultipleResult()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Country result = null;

            // Act
            Assert.Throws<InvalidOperationException>(() => result = tableSet.Where(p => p.Population > 20000000).Single());

            // Assert
            Assert.Null(result);
        }

        // ReSharper restore ReplaceWithSingleCallToSingle

        [IntegrationFact]
        public void SingleAfterWhereWithExpression()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country result = tableSet.Where(p => p.Population > 20000000).Single(p => p.Population < 30000000);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(Yugoslavia, result.Name);
        }
    }
}