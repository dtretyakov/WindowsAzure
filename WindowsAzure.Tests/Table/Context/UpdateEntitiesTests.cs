using System;
using System.Collections.Generic;
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
    public sealed class UpdateEntitiesTests
    {
        [Fact]
        public void UpdateEntities()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            IList<Country> countries = ObjectsFactory.GetCountries();

            // Act
            var result = context.Update(countries);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteBatches(countries, TableOperation.Replace), Times.Once());
            Assert.Equal(countries, result);
        }

        [Fact]
        public void UpdateEntitiesWithNullParameter()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            IEnumerable<Country> result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { result = context.Update((IEnumerable<Country>)null); });

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IEnumerable<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public void UpdateEmptyCollection()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
            {
                RequestExecutor = mock.Object
            };

            var countries = new List<Country>();

            // Act
            var result = context.Update(countries);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(countries, result);
        }

        [Fact]
        public async Task UpdateEntitiesAsync()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            var countries = ObjectsFactory.GetCountries();

            // Act
            var result = await context.UpdateAsync(countries);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteBatchesAsync(countries, TableOperation.Replace, It.IsAny<CancellationToken>()));
            Assert.Equal(countries, result);
        }

        [Fact]
        public async Task UpdateEntitiesWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    RequestExecutor = mock.Object
                };

            IEnumerable<Country> result = null;

            // Act
            try
            {
                result = await context.UpdateAsync((IEnumerable<Country>)null, CancellationToken.None);
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IEnumerable<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task UpdateEmptyCollectionAsync()
        {
            // Arrange
            Mock<ITableRequestExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
            {
                RequestExecutor = mock.Object
            };

            var countries = new List<Country>();

            // Act
            var result = await context.AddAsync(countries);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(countries, result);
        }
    }
}