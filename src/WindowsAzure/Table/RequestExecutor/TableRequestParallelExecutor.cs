using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Wrappers;

namespace WindowsAzure.Table.RequestExecutor
{
    /// <summary>
    ///     Handles query execution.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class TableRequestParallelExecutor<T> : TableRequestExecutorBase<T> where T : new()
    {
        private readonly ICloudTable _cloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;
        private readonly ITableBatchPartitioner _partitioner;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        /// <param name="partitioner">Batch partitioner.</param>
        internal TableRequestParallelExecutor(ICloudTable cloudTable, ITableEntityConverter<T> entityConverter, ITableBatchPartitioner partitioner)
            : base(cloudTable, entityConverter)
        {
            if (partitioner == null)
            {
                throw new ArgumentNullException("partitioner");
            }

            _cloudTable = cloudTable;
            _entityConverter = entityConverter;
            _partitioner = partitioner;
        }

        /// <summary>
        ///     Executes batch operations.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Result entities.</returns>
        public override IEnumerable<T> ExecuteBatches(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

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
        ///     Executes batch operations without returning results.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// >
        public override void ExecuteBatchesWithoutResult(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            IEnumerable<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p));
            IEnumerable<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);

            Parallel.ForEach(batches, batch => _cloudTable.ExecuteBatch(batch));
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
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            IEnumerable<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p));
            IEnumerable<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);

            IEnumerable<Task> batchExecutionTasks = batches.Select(
                p => _cloudTable.ExecuteBatchAsync(p, cancellationToken)
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
                    }, cancellationToken);
        }

        /// <summary>
        ///     Executes batch operations without returning results asynchronously.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public override Task ExecuteBatchesWithoutResultAsync(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            if (operation == null)
            {
                throw new ArgumentNullException("operation");
            }

            IEnumerable<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p));
            IEnumerable<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);
            IEnumerable<Task> batchExecutionTasks = batches.Select(p => _cloudTable.ExecuteBatchAsync(p, cancellationToken));

            return Task.Factory.ContinueWhenAll(
                batchExecutionTasks.ToArray(),
                tasks =>
                    {
                        List<AggregateException> exceptions = tasks
                            .Where(p => p.Exception != null)
                            .Select(p => p.Exception).ToList();

                        if (exceptions.Count > 0)
                        {
                            return TaskHelpers.FromErrors(exceptions);
                        }

                        return TaskHelpers.Completed();
                    }, cancellationToken);
        }
    }
}