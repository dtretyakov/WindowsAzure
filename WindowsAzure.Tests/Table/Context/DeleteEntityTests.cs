using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table;
using WindowsAzure.Table.RequestExecutor;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class DeleteEntityTests
    {
        [Fact]
        public void RemoveEntity()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            Country country = ObjectsFactory.GetCountry();

            // Act
            context.Remove(country);

            // Assert
            mock.Verify(executor => executor.ExecuteWithoutResult(country, TableOperation.Delete), Times.Once());
        }

        [Fact]
        public void RemoveEntityWithNullParameter()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            // Act
            Assert.Throws<ArgumentNullException>(() => context.Remove((Country) null));

            // Assert
            mock.Verify(executor => executor.Execute(It.IsAny<Country>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task RemoveEntityAsync()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            Country country = ObjectsFactory.GetCountry();

            // Act
            await context.RemoveAsync(country);

            // Assert
            mock.Verify(executor => executor.ExecuteWithoutResultAsync(country, TableOperation.Delete, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async Task RemoveEntityWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            // Act
            try
            {
                await context.RemoveAsync((Country) null, CancellationToken.None);
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            mock.Verify(executor => executor.Execute(It.IsAny<Country>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }
    }
}