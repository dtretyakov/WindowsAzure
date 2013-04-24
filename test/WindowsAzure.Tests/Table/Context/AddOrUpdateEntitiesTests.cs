using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table;
using WindowsAzure.Table.QueryExecutor;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class AddOrUpdateEntitiesTests
    {
        [Fact]
        public void AddOrUpdateEntities()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            var countries = ObjectsFactory.GetCountries();

            // Act
            var result = context.AddOrUpdate(countries);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteBatches(countries, TableOperation.InsertOrReplace), Times.Once());
            Assert.Equal(countries, result);
        }

        [Fact]
        public void AddOrUpdateEntitiesWithNullParameter()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            IEnumerable<Country> result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { result = context.AddOrUpdate((IEnumerable<Country>)null); });

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IEnumerable<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public void AddOrUpdateEmptyCollection()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
            {
                QueryExecutor = mock.Object
            };

            var countries = new List<Country>();

            // Act
            var result = context.AddOrUpdate(countries);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(countries, result);
        }

        [Fact]
        public async Task AddOrUpdateEntitiesAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            var countries = ObjectsFactory.GetCountries();

            // Act
            var result = await context.AddOrUpdateAsync(countries);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteBatchesAsync(countries, TableOperation.InsertOrReplace, It.IsAny<CancellationToken>()));
            Assert.Equal(countries, result);
        }

        [Fact]
        public async Task AddOrUpdateEntitiesWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            IEnumerable<Country> result = null;

            // Act
            try
            {
                result = await context.AddOrUpdateAsync((IEnumerable<Country>)null, CancellationToken.None);
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IEnumerable<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task AddOrUpdateEmptyCollectionAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
            {
                QueryExecutor = mock.Object
            };

            var countries = new List<Country>();

            // Act
            var result = await context.AddOrUpdateAsync(countries);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(countries, result);
        }
    }
}