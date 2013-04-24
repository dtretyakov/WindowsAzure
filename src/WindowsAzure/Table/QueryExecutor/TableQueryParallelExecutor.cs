using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;

namespace WindowsAzure.Table.QueryExecutor
{
    /// <summary>
    ///     Handles query execution.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class TableQueryParallelExecutor<T> : TableQueryExecutorBase<T> where T : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;
        private readonly TableBatchPartitioner _partitioner;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        internal TableQueryParallelExecutor(CloudTable cloudTable, ITableEntityConverter<T> entityConverter)
            : base(cloudTable, entityConverter)
        {
            _cloudTable = cloudTable;
            _entityConverter = entityConverter;
            _partitioner = new TableBatchPartitioner();
        }

        /// <summary>
        ///     Executes batch operations.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Result entities.</returns>
        public override IEnumerable<T> ExecuteBatches(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation)
        {
            IEnumerable<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p));
            IEnumerable<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);

            return batches.AsParallel().SelectMany(GetEntities);
        }

        /// <summary>
        ///     Retrieves & convert entities from cloud table.
        /// </summary>
        /// <param name="batch">Batch operation.</param>
        /// <returns>Result entities.</returns>
        private IEnumerable<T> GetEntities(TableBatchOperation batch)
        {
            return _cloudTable
                .ExecuteBatch(batch)
                .Select(p => _entityConverter.GetEntity((DynamicTableEntity) p.Result));
        }

        /// <summary>
        ///     Executes batch operations asynchronously.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entities.</returns>
        public override Task<IEnumerable<T>> ExecuteBatchesAsync(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken)
        {
            IEnumerable<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p));
            IEnumerable<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);

            IEnumerable<Task> batchExecutionTasks = batches.Select(
                p => _cloudTable
                         .ExecuteBatchAsync(p, cancellationToken)
                         .Then(results => results.Select(
                             q => _entityConverter.GetEntity((DynamicTableEntity) q.Result)), cancellationToken));

            return Task.Factory.ContinueWhenAll(
                batchExecutionTasks.ToArray(),
                tasks =>
                    {
                        var result = new List<T>(tasks.Length);

                        foreach (Task task in tasks)
                        {
                            var typedTask = (Task<IEnumerable<T>>) task;
                            result.AddRange(typedTask.Result);
                        }

                        return result.AsEnumerable();
                    },
                cancellationToken);
        }
    }
}