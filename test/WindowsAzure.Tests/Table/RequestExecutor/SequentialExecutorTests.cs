using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.RequestExecutor;
using WindowsAzure.Table.Wrappers;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.RequestExecutor
{
    public sealed class SequentialExecutorTests
    {
        [Fact]
        public void CreateExecutorWithNullCloudTableParameter()
        {
            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableRequestSequentialExecutor<Country>(null, null, null));
        }

        [Fact]
        public void CreateExecutorWithNullEntityConverterParameter()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, null, null));
        }

        [Fact]
        public void CreateExecutorWithNullBatchPartitioner()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, null));
        }

        [Fact]
        public void CreateExecutor()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();

            // Act
            var result = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ExecuteBatchesWithNullEntities()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatches(null, null));
        }
        [Fact]
        public void ExecuteBatchesWithNullOperation()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatches(entities, null));
        }

        [Fact]
        public void ExecuteBatches()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act
            var result = executor.ExecuteBatches(entities, TableOperation.Insert).ToList();

            // Assert
            Assert.IsAssignableFrom<IEnumerable<Country>>(result);
            entityConverterMock.Verify(p => p.GetEntity(It.IsAny<Country>()), Times.Exactly(2));
            batchPartitionerMock.Verify(p => p.GetBatches(It.IsAny<IEnumerable<ITableEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Once());
        }

        [Fact]
        public void ExecuteBatchesAsyncWithNullEntities()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatchesAsync(null, null, CancellationToken.None));
        }
        [Fact]
        public void ExecuteBatchesAsyncWithNullOperation()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatchesAsync(entities, null, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteBatchesAsync()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act
            var result = await executor.ExecuteBatchesAsync(entities, TableOperation.Insert, CancellationToken.None);

            // Assert
            Assert.IsAssignableFrom<IEnumerable<Country>>(result);
            entityConverterMock.Verify(p => p.GetEntity(It.IsAny<Country>()), Times.Exactly(2));
            batchPartitionerMock.Verify(p => p.GetBatches(It.IsAny<IEnumerable<ITableEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Once());
        }

        [Fact]
        public void ExecuteBatchesWithoutResultWithNullEntities()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatchesWithoutResult(null, null));
        }
        [Fact]
        public void ExecuteBatchesWithoutResultWithNullOperation()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatchesWithoutResult(entities, null));
        }

        [Fact]
        public void ExecuteBatchesWithoutResult()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act
            executor.ExecuteBatchesWithoutResult(entities, TableOperation.Insert);

            // Assert
            entityConverterMock.Verify(p => p.GetEntity(It.IsAny<Country>()), Times.Exactly(2));
            batchPartitionerMock.Verify(p => p.GetBatches(It.IsAny<IEnumerable<ITableEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Once());
        }

        [Fact]
        public void ExecuteBatchesWithoutResultAsyncWithNullEntities()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatchesWithoutResultAsync(null, null, CancellationToken.None));
        }
        [Fact]
        public void ExecuteBatchesWithoutResultAsyncWithNullOperation()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => executor.ExecuteBatchesWithoutResultAsync(entities, null, CancellationToken.None));
        }

        [Fact]
        public async Task ExecuteBatchesWithoutResultAsync()
        {
            // Arrange
            Mock<ICloudTable> cloudTableMock = MocksFactory.GetCloudTableMock();
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();
            Mock<ITableBatchPartitioner> batchPartitionerMock = MocksFactory.GetTableBatchPartitionerMock();
            var executor = new TableRequestSequentialExecutor<Country>(cloudTableMock.Object, entityConverterMock.Object, batchPartitionerMock.Object);
            var entities = ObjectsFactory.GetCountries();

            // Act
            await executor.ExecuteBatchesWithoutResultAsync(entities, TableOperation.Insert, CancellationToken.None);

            // Assert
            entityConverterMock.Verify(p => p.GetEntity(It.IsAny<Country>()), Times.Exactly(2));
            batchPartitionerMock.Verify(p => p.GetBatches(It.IsAny<IEnumerable<ITableEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()), Times.Once());
        }
    }
}