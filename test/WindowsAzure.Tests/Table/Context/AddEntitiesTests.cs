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
    public sealed class AddEntitiesTests
    {
        [Fact]
        public void AddEntities()
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
            IList<Country> result = context.Add(countries);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteBatches(countries, TableOperation.Insert), Times.Once());
            Assert.Equal(countries, result);
        }

        [Fact]
        public void AddEntitiesWithNullParameter()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            IList<Country> result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { result = context.Add((IList<Country>) null); });

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IList<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task AddEntitiesAsync()
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
            IList<Country> result = await context.AddAsync(countries);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteBatchesAsync(countries, TableOperation.Insert, It.IsAny<CancellationToken>()));
            Assert.Equal(countries, result);
        }

        [Fact]
        public async Task AddEntitiesWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            IList<Country> result = null;

            // Act
            try
            {
                result = await context.AddAsync((IList<Country>) null, CancellationToken.None);
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.ExecuteBatches(It.IsAny<IList<Country>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }
    }
}