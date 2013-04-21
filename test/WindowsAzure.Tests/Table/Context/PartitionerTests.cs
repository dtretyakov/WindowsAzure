using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class PartitionerTests
    {
        [Fact]
        public void CreateBatches()
        {
            // Arrange
            var configuration = new TableSetConfiguration
                {
                    PartitioningMode = PartitioningMode.None
                };
            var partitioner = new TableBatchPartitioner(configuration);
            const int count = 200;
            IEnumerable<ITableEntity> entities = Enumerable.Range(0, count)
                .Select(p => (ITableEntity)new DynamicTableEntity(string.Empty, string.Format("PK{0}", p)));

            // Act
            IList<TableBatchOperation> batches = partitioner.GetBatches(entities.ToList(), TableOperation.Insert);

            // Assert
            Assert.NotNull(batches);
            Assert.Equal(2, batches.Count);
        }

        [Fact]
        public void CreateGroupedBatchesWith2PartitionKeys()
        {
            // Arrange
            var configuration = new TableSetConfiguration
            {
                PartitioningMode = PartitioningMode.Sequential
            };
            var partitioner = new TableBatchPartitioner(configuration);
            const int count = 300;
            IEnumerable<ITableEntity> entities = Enumerable.Range(0, count)
                .Select(p => (ITableEntity)new DynamicTableEntity(string.Format("PK{0}", p % 2), p.ToString(CultureInfo.InvariantCulture)));

            // Act
            IList<TableBatchOperation> batches = partitioner.GetBatches(entities.ToList(), TableOperation.Insert);

            // Assert
            Assert.NotNull(batches);
            Assert.Equal(count, batches.Sum(p => p.Count));
            Assert.Equal(100, batches[0].Count);
            Assert.Equal(50, batches[1].Count);
            Assert.Equal(100, batches[2].Count);
            Assert.Equal(50, batches[3].Count);
        }
    }
}