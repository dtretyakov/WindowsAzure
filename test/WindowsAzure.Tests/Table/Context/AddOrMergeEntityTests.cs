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
    public sealed class AddOrMergeEntityTests
    {
        [Fact]
        public void AddOrMergeEntity()
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
            Country result = context.AddOrMerge(country);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.Execute(country, TableOperation.InsertOrMerge), Times.Once());
            Assert.Equal(country, result);
        }

        [Fact]
        public void AddOrMergeEntityWithNullParameter()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            Country result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { result = context.AddOrMerge((Country) null); });

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.Execute(It.IsAny<Country>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task AddOrMergeEntityAsync()
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
            Country result = await context.AddOrMergeAsync(country);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteAsync(country, TableOperation.InsertOrMerge, It.IsAny<CancellationToken>()));
            Assert.Equal(country, result);
        }

        [Fact]
        public async Task AddOrMergeEntityWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            Country result = null;

            // Act
            try
            {
                result = await context.AddOrMergeAsync((Country) null, CancellationToken.None);
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.Execute(It.IsAny<Country>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }
    }
}