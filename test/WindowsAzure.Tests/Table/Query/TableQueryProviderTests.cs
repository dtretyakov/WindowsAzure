using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Query
{
    public sealed class TableQueryProviderTests : CountryTableSetBase
    {
        private const string Germany = "Germany";
        private const string Spain = "Spain";
        private const string Finland = "Finland";
        private const string France = "France";


        public TableQueryProviderTests()
        {
            TableSet<Country> tableSet = GetTableSet();
            tableSet.Add(
                new List<Country>
                    {
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
                            },
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
                            },
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
                            },
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
                            }
                    }
                );
        }

        /// <summary>
        ///     Creates a TableQueryProvider instance.
        /// </summary>
        /// <returns>TableQueryProvider.</returns>
        private TableQueryProvider<Country> GetTableQueryProvider()
        {
            return new TableQueryProvider<Country>(
                GenerateCloudTableClient(),
                TableName,
                new TableEntityConverter<Country>());
        }

        [Fact]
        public void CallExecuteMethodOfQueryProviderTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            TableQueryProvider<Country> queryProvider = GetTableQueryProvider();
            IQueryable<Country> query = tableSet.Where(p => p.IsExists);

            // Act
            object result = queryProvider.Execute(query.Expression);

            //Assert
            Assert.NotNull(result);
            Assert.True(result is IEnumerable<Country>);

            List<Country> typedResult = ((IEnumerable<Country>) result).ToList();
            Assert.Equal(typedResult.Count, 3);

            List<string> names = typedResult.Select(p => p.Name).ToList();
            Assert.Contains(Germany, names);
            Assert.Contains(France, names);
            Assert.Contains(Finland, names);
        }

        [Fact]
        public async Task CallExecuteAsyncMethodOfQueryProviderTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            TableQueryProvider<Country> queryProvider = GetTableQueryProvider();
            IQueryable<Country> query = tableSet.Where(p => p.IsExists == false);

            // Act
            object result = await queryProvider.ExecuteAsync(query.Expression);

            //Assert
            Assert.NotNull(result);
            Assert.True(result is IEnumerable<Country>);

            List<Country> typedResult = ((IEnumerable<Country>) result).ToList();
            Assert.Equal(typedResult.Count, 1);
            Assert.Equal(typedResult[0].Name, Spain);
        }

        [Fact]
        public void CallGetTableQueryMethodOfQueryProviderTest()
        {
            // Arrange
            TableSet<Country> tableSet = GetTableSet();
            TableQueryProvider<Country> queryProvider = GetTableQueryProvider();
            IQueryable<Country> query = tableSet.Where(p => p.IsExists).Take(2);

            // Act
            TableQuery result = queryProvider.GetTableQuery(query.Expression);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result.FilterString, "IsExists");
            Assert.Equal(result.SelectColumns, null);
            Assert.True(result.TakeCount.HasValue);
// ReSharper disable PossibleInvalidOperationException
            Assert.Equal(result.TakeCount.Value, 2);
// ReSharper restore PossibleInvalidOperationException
        }
    }
}