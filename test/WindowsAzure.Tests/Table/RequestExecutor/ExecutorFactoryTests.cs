using System;
using System.ComponentModel;
using Moq;
using WindowsAzure.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.RequestExecutor;
using WindowsAzure.Table.Wrappers;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.RequestExecutor
{
    public sealed class ExecutorFactoryTests
    {
        [Fact]
        public void CreateParallelExecutor()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            var executorFactory = new TableRequestExecutorFactory<Country>(cloudTableMock.Object, entityConverterMock.Object);

            // Act
            ITableRequestExecutor<Country> executor = executorFactory.Create(ExecutionMode.Parallel);

            // Assert
            Assert.NotNull(executor);
            Assert.IsType<TableRequestParallelExecutor<Country>>(executor);
            Assert.Equal(cloudTableMock.Object, executorFactory.CloudTable);
        }

        [Fact]
        public void CreateSequentialExecutor()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            var executorFactory = new TableRequestExecutorFactory<Country>(cloudTableMock.Object, entityConverterMock.Object);

            // Act
            ITableRequestExecutor<Country> executor = executorFactory.Create(ExecutionMode.Sequential);

            // Assert
            Assert.NotNull(executor);
            Assert.IsType<TableRequestSequentialExecutor<Country>>(executor);
            Assert.Equal(cloudTableMock.Object, executorFactory.CloudTable);
        }

        [Fact]
        public void CreateExecutorWithNullCloudTableParameter()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableRequestExecutorFactory<Country>(null, entityConverterMock.Object));
        }

        [Fact]
        public void CreateExecutorWithNullEntityConverterParameter()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableRequestExecutorFactory<Country>(cloudTableMock.Object, null));
        }

        [Fact]
        public void CreateExecutorWithInvalidMode()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            var executorFactory = new TableRequestExecutorFactory<Country>(cloudTableMock.Object, entityConverterMock.Object);
            ITableRequestExecutor<Country> executor = null;

            // Act
            Assert.Throws<InvalidEnumArgumentException>(() => executor = executorFactory.Create((ExecutionMode) 2));

            // Assert
            Assert.Null(executor);
        }
    }
}