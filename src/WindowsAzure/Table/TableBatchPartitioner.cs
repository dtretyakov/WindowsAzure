using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;

namespace WindowsAzure.Table
{
    /// <summary>
    ///     Handles entities batching in insert operations.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894038.aspx
    /// </summary>
    internal sealed class TableBatchPartitioner
    {
        private const int BatchCount = 100;
        private readonly TableSetConfiguration _configuration;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        public TableBatchPartitioner(TableSetConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        ///     Generates a list of batches according to partitioning settings.
        /// </summary>
        /// <param name="tableEntities">Entities collection.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Batches.</returns>
        public IList<TableBatchOperation> GetBatches(IList<ITableEntity> tableEntities, Func<ITableEntity, TableOperation> operation)
        {
            if (tableEntities == null)
            {
                throw new ArgumentNullException("tableEntities");
            }

            if (tableEntities.Count == 0)
            {
                throw new ArgumentException(Resources.TableBatchPartitionerNoEntities, "tableEntities");
            }

            if (_configuration.PartitioningMode == PartitioningMode.None)
            {
                return GetWithoutPartitioning(tableEntities, operation);
            }

            return GetWithPartitioning(tableEntities, operation);
        }

        private IList<TableBatchOperation> GetWithPartitioning(IList<ITableEntity> tableEntities, Func<ITableEntity, TableOperation> operation)
        {
            var result = new List<TableBatchOperation>(tableEntities.Count/BatchCount);
            IEnumerable<IGrouping<string, ITableEntity>> groupedEntites = tableEntities.GroupBy(p => p.PartitionKey);

            foreach (var group in groupedEntites)
            {
                result.AddRange(GetWithoutPartitioning(group.ToList(), operation));
            }

            return result;
        }

        private IList<TableBatchOperation> GetWithoutPartitioning(IList<ITableEntity> tableEntities, Func<ITableEntity, TableOperation> operation)
        {
            var result = new List<TableBatchOperation>(tableEntities.Count/BatchCount);
            var batchOperation = new TableBatchOperation();

            foreach (ITableEntity entity in tableEntities)
            {
                batchOperation.Add(operation(entity));

                if (batchOperation.Count == BatchCount)
                {
                    result.Add(batchOperation);
                    batchOperation = new TableBatchOperation();
                }
            }

            if (batchOperation.Count > 0)
            {
                result.Add(batchOperation);
            }

            return result;
        }
    }
}