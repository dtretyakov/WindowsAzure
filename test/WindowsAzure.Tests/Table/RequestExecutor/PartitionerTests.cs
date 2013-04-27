using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.RequestExecutor;
using Xunit;

namespace WindowsAzure.Tests.Table.RequestExecutor
{
    public sealed class PartitionerTests
    {
        [Fact]
        public void CreateBatchesFromSequenceWithSamePartiotionKey()
        {
            // Arrange
            var partitioner = new TableBatchPartitioner();
            const int count = 200;
            IEnumerable<ITableEntity> entities = Enumerable
                .Range(0, count)
                .Select(p => (ITableEntity) new DynamicTableEntity(string.Empty, string.Format("PK{0}", p)));

            // Act
            List<TableBatchOperation> batches = partitioner.GetBatches(entities, TableOperation.Insert).ToList();

            // Assert
            Assert.NotNull(batches);
            Assert.Equal(2, batches.Count);
        }

        [Fact]
        public void CreateBatchesFromShuffledSequenceWith2PartitionKeys()
        {
            // Arrange
            var partitioner = new TableBatchPartitioner();
            const int count = 300;
            IEnumerable<ITableEntity> entities = Enumerable
                .Range(0, count)
                .Select(p => (ITableEntity) new DynamicTableEntity(string.Format("PK{0}", p%2), p.ToString(CultureInfo.InvariantCulture)));

            // Act
            List<TableBatchOperation> batches = partitioner.GetBatches(entities, TableOperation.Insert).ToList();

            // Assert
            Assert.NotNull(batches);
            Assert.Equal(count, batches.Sum(p => p.Count));

            var fieldInfo = typeof(TableBatchOperation).GetField("partitionKey", BindingFlags.NonPublic | BindingFlags.Instance);

            var aggregateResult = batches.Select(p => fieldInfo != null ? new
                {
                    partitonKey = (string)fieldInfo.GetValue(p),
                    count = p.Count
                } : null)
                .OrderBy(p => p.partitonKey)
                .GroupBy(p => p.partitonKey)
                .ToList();

            Assert.Equal("PK0", aggregateResult[0].Key);
            Assert.Equal("PK1", aggregateResult[1].Key);
            Assert.Equal(100, aggregateResult[0].ElementAt(0).count);
            Assert.Equal(50, aggregateResult[0].ElementAt(1).count);
            Assert.Equal(100, aggregateResult[1].ElementAt(0).count);
            Assert.Equal(50, aggregateResult[1].ElementAt(1).count);
        }

        [Fact]
        public void CreateBatchesFromSequencWithDifferentPartitionKeys()
        {
            // Arrange
            var partitioner = new TableBatchPartitioner();
            const int count = 200;
            IEnumerable<ITableEntity> entities = Enumerable
                .Range(0, count)
                .Select(p => (ITableEntity)new DynamicTableEntity(string.Format("PK{0}", p), string.Empty));

            // Act
            List<TableBatchOperation> batches = partitioner.GetBatches(entities, TableOperation.Insert).ToList();

            // Assert
            Assert.NotNull(batches);
            Assert.Equal(200, batches.Count);
        }

        [Fact]
        public void CreateBatchesWithNullEntitiesParameter()
        {
            // Arrange
            var partitioner = new TableBatchPartitioner();
            List<TableBatchOperation> batches = null;

            // Act
            Assert.Throws<ArgumentNullException>(() =>
                {
                    batches = partitioner.GetBatches(null, TableOperation.Insert).ToList();
                });

            // Assert
            Assert.Null(batches);
        }

        [Fact]
        public void CreateBatchesWithNullOperationParameter()
        {
            // Arrange
            var partitioner = new TableBatchPartitioner();
            List<TableBatchOperation> batches = null;

            // Act
            Assert.Throws<ArgumentNullException>(() =>
            {
                batches = partitioner.GetBatches(new List<ITableEntity>(), null).ToList();
            });

            // Assert
            Assert.Null(batches);
        }
    }
}