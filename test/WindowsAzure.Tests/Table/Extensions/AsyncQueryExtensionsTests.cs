using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsAzure.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Extensions
{
    public sealed class AsyncQueryExtensionsTests : TableSetTestBase
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private const string Finland = "Finland";
        private const string France = "France";

        public AsyncQueryExtensionsTests()
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
        public async Task RetreiveWithToListAsyncTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            List<Country> entities = await tableSet.ToListAsync();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, 4);

            List<string> names = entities.Select(p => p.Name).ToList();
            Assert.Contains(Germany, names);
            Assert.Contains(Spain, names);
            Assert.Contains(France, names);
            Assert.Contains(Finland, names);
        }

        [Fact]
        public async Task RetreiveWithTakeAsyncTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            const int count = 2;

            // Act
            List<Country> entities = (await tableSet.TakeAsync(count)).ToList();

            // Assert
            Assert.NotNull(entities);
            Assert.Equal(entities.Count, count);
        }

        [Fact]
        public async Task RetreiveWithFirstAsyncTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.Where(p => p.Name == France).FirstAsync();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, France);
        }

        [Fact]
        public async Task RetreiveWithFirstAsyncAndPredicateTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.FirstAsync(p => p.Name == Spain);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, Spain);
        }

        [Fact]
        public async Task RetreiveWithFirstOrDefaultAsyncTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.Where(p => p.Name == Germany).FirstOrDefaultAsync();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, Germany);
        }

        [Fact]
        public async Task RetreiveWithFirstOrDefaultAsyncAndPredicateTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.FirstOrDefaultAsync(p => p.Name == Germany);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, Germany);
        }

        [Fact]
        public async Task RetreiveWithFirstOrDefaultAsyncAndPredicateWithInvalidNameTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.FirstOrDefaultAsync(p => p.Name == "none");

            // Assert
            Assert.Null(entity);
        }

        [Fact]
        public async Task RetreiveWithSingleAsyncTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.Where(p => p.Name == Finland).SingleAsync();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, Finland);
        }

        [Fact]
        public async Task RetreiveWithSingleAsyncWithMultipleResultsTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Exception exception = null;

            // Act
            try
            {
                await tableSet.SingleAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }

            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public async Task RetreiveWithSingleAsyncAndPredicateTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.SingleAsync(p => p.Name == Finland);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, Finland);
        }

        [Fact]
        public async Task RetreiveWithSingleOrDefaultAsyncTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.Where(p => p.Name == France).SingleOrDefaultAsync();

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, France);
        }

        [Fact]
        public async Task RetreiveWithSingleOrDefaultAsyncWithMultipleResultsTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            Exception exception = null;

            // Act
            try
            {
                await tableSet.SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                exception = e;
            }
            
            // Assert
            Assert.NotNull(exception);
            Assert.IsType<InvalidOperationException>(exception);
        }

        [Fact]
        public async Task RetreiveWithSingleOrDefaultAsyncAndPredicateTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.SingleOrDefaultAsync(p => p.Name == France);

            // Assert
            Assert.NotNull(entity);
            Assert.Equal(entity.Name, France);
        }

        [Fact]
        public async Task RetreiveWithSingleOrDefaultAsyncAndPredicateWithInvalidNameTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();

            // Act
            Country entity = await tableSet.SingleOrDefaultAsync(p => p.Name == "none");

            // Assert
            Assert.Null(entity);
        }
    }
}