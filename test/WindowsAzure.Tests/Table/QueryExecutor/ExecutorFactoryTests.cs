using System;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.QueryExecutor;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.QueryExecutor
{
    public sealed class ExecutorFactoryTests
    {
        [Fact]
        public void CreateParallelExecutor()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            CloudTable cloudTable = ObjectsFactory.GetCloudTable();
            var executorFactory = new TableQueryExecutorFactory<Country>(cloudTable, entityConverterMock.Object);

            // Act
            ITableQueryExecutor<Country> executor = executorFactory.Create(ExecutionMode.Parallel);

            // Assert
            Assert.NotNull(executor);
            Assert.IsType<TableQueryParallelExecutor<Country>>(executor);
            Assert.Equal(cloudTable, executorFactory.CloudTable);
        }

        [Fact]
        public void CreateSequentialExecutor()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            CloudTable cloudTable = ObjectsFactory.GetCloudTable();
            var executorFactory = new TableQueryExecutorFactory<Country>(cloudTable, entityConverterMock.Object);

            // Act
            ITableQueryExecutor<Country> executor = executorFactory.Create(ExecutionMode.Sequential);

            // Assert
            Assert.NotNull(executor);
            Assert.IsType<TableQuerySequentialExecutor<Country>>(executor);
            Assert.Equal(cloudTable, executorFactory.CloudTable);
        }

        [Fact]
        public void CreateExecutorWithNullCloudTableParameter()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableQueryExecutorFactory<Country>(null, entityConverterMock.Object));
        }

        [Fact]
        public void CreateExecutorWithNullEntityConverterParameter()
        {
            // Arrange
            CloudTable cloudTable = ObjectsFactory.GetCloudTable();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableQueryExecutorFactory<Country>(cloudTable, null));
        }
    }
}