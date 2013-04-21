using System;
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
    public sealed class AddEntityTests
    {
        [Fact]
        public void AddEntity()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            Country country = ObjectsFactory.GetCountry();

            // Act
            Country result = context.Add(country);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.Execute(country, TableOperation.Insert), Times.Once());
            Assert.Equal(country, result);
        }

        [Fact]
        public void AddEntityWithNullParameter()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            Country result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => { result = context.Add((Country) null); });

            // Assert
            Assert.Null(result);
            mock.Verify(executor => executor.Execute(It.IsAny<Country>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Never());
        }

        [Fact]
        public async Task AddEntityAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            Country country = ObjectsFactory.GetCountry();

            // Act
            Country result = await context.AddAsync(country);

            // Assert
            Assert.NotNull(result);
            mock.Verify(executor => executor.ExecuteAsync(country, TableOperation.Insert, It.IsAny<CancellationToken>()));
            Assert.Equal(country, result);
        }

        [Fact]
        public async Task AddEntityWithNullParameterAsync()
        {
            // Arrange
            Mock<ITableQueryExecutor<Country>> mock = MocksFactory.GetQueryExecutorMock<Country>();
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient)
                {
                    QueryExecutor = mock.Object
                };

            Country result = null;

            // Act
            try
            {
                result = await context.AddAsync((Country) null, CancellationToken.None);
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