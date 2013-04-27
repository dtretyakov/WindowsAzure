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
    internal sealed class TableQuerySequentialExecutor<T> : TableQueryExecutorBase<T> where T : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;
        private readonly TableBatchPartitioner _partitioner;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        internal TableQuerySequentialExecutor(CloudTable cloudTable, ITableEntityConverter<T> entityConverter)
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

            return from batch in batches
                   from result in _cloudTable.ExecuteBatch(batch)
                   select _entityConverter.GetEntity((DynamicTableEntity) result.Result);
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

            var result = new List<T>();

            IEnumerable<Task> tasks = batches.Select(
                p => _cloudTable
                         .ExecuteBatchAsync(p, cancellationToken)
                         .Then(results => result.AddRange(results.Select(
                             tableResult => _entityConverter.GetEntity((DynamicTableEntity) tableResult.Result))), cancellationToken));

            return TaskHelpers.Iterate(tasks, cancellationToken)
                              .Then(() => result.AsEnumerable(), cancellationToken);
        }
    }
}