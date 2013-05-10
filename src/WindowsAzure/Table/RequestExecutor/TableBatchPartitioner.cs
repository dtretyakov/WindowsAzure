using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.RequestExecutor
{
    /// <summary>
    ///     Handles entities batching in insert operations.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894038.aspx
    /// </summary>
    internal sealed class TableBatchPartitioner : ITableBatchPartitioner
    {
        private const int BatchCount = 100;

        /// <summary>
        ///     Generates a list of batches according to partitioning settings.
        /// </summary>
        /// <param name="tableEntities">Entities collection.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Batches.</returns>
        public IEnumerable<TableBatchOperation> GetBatches(IEnumerable<ITableEntity> tableEntities, Func<ITableEntity, TableOperation> operation)
        {
            if (tableEntities == null)
            {
                throw new ArgumentNullException("tableEntities");
            }

            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            var batches = new Dictionary<string, TableBatchOperation>();

            foreach (ITableEntity tableEntity in tableEntities)
            {
                TableBatchOperation batch;

                if (!batches.TryGetValue(tableEntity.PartitionKey, out batch))
                {
                    batch = new TableBatchOperation();
                    batches.Add(tableEntity.PartitionKey, batch);
                }

                batch.Add(operation(tableEntity));

                if (batch.Count == BatchCount)
                {
                    batches.Remove(tableEntity.PartitionKey);
                    yield return batch;
                }
            }

            foreach (var pair in batches)
            {
                yield return pair.Value;
            }
        }
    }
}