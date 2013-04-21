using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class DeleteEntitiesTests
    {
        [Fact]
        public void RemoveEntities()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            IList<Country> countries = ObjectsFactory.GetCountries();

            // Act
            context.Remove(countries);

            // Assert
            mock.Verify(executor => executor.ExecuteBatches(countries, TableOperation.Delete), Times.Once());
        }

        [Fact]
        public void RemoveEntitiesWithNullParameter()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            // Act
            Assert.Throws<ArgumentNullException>(() => context.Remove((IList<Country>) null));

            // Assert
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IList<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task RemoveEntitiesAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            IList<Country> countries = ObjectsFactory.GetCountries();

            // Act
            await context.RemoveAsync(countries);

            // Assert
            mock.Verify(executor => executor.ExecuteBatchesAsync(countries, TableOperation.Delete, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task RemoveEntitiesWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            // Act
            try
            {
                await context.RemoveAsync((IList<Country>) null, CancellationToken.None);
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IList<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }
    }
}