using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;

namespace WindowsAzure.Table
{
    /// <summary>
    ///     Handles execution of single or batch queries.
    /// </summary>
    internal sealed class TableQueryExecutor<TEntity> : ITableQueryExecutor<TEntity> where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly TableSetConfiguration _configuration;
        private readonly ITableEntityConverter<TEntity> _entityConverter;
        private readonly TableBatchPartitioner _partitioner;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        /// <param name="configuration">Configuration.</param>
        public TableQueryExecutor(CloudTable cloudTable, ITableEntityConverter<TEntity> entityConverter, TableSetConfiguration configuration)
        {
            _cloudTable = cloudTable;
            _entityConverter = entityConverter;
            _configuration = configuration;
            _partitioner = new TableBatchPartitioner(_configuration);
        }

        /// <summary>
        ///     Executes operation.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <returns>Result entity.</returns>
        public TEntity Execute(TEntity entity, Func<ITableEntity, TableOperation> operation)
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);
            TableResult result = _cloudTable.Execute(operation(tableEntity));

            return _entityConverter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Executes operation asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entity.</returns>
        public Task<TEntity> ExecuteAsync(TEntity entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken)
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            return _cloudTable
                .ExecuteAsync(operation(tableEntity), cancellationToken)
                .Then(result =>
                    {
                        var value = (DynamicTableEntity) result.Result;
                        return _entityConverter.GetEntity(value);
                    }, cancellationToken);
        }

        /// <summary>
        ///     Executes batch operations.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Result entities.</returns>
        public IList<TEntity> ExecuteBatches(IList<TEntity> entities, Func<ITableEntity, TableOperation> operation)
        {
            List<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p)).ToList();
            IList<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);

            switch (_configuration.PartitioningMode)
            {
                case PartitioningMode.None:
                case PartitioningMode.Sequential:
                    return ExecuteBatchesSequentially(batches);

                case PartitioningMode.Parallel:
                    return ExecuteBatchesParallel(batches);

                default:
                    string message = string.Format(Resources.TableQueryExecutorInvalidPartitioningMode, _configuration.PartitioningMode);
                    throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        ///     Executes batch operations asynchronously.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entities.</returns>
        public Task<IList<TEntity>> ExecuteBatchesAsync(IList<TEntity> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken)
        {
            List<ITableEntity> tableEntities = entities.Select(p => _entityConverter.GetEntity(p)).ToList();
            IList<TableBatchOperation> batches = _partitioner.GetBatches(tableEntities, operation);

            switch (_configuration.PartitioningMode)
            {
                case PartitioningMode.None:
                case PartitioningMode.Sequential:
                    return ExecuteBatchesSequentiallyAsync(batches, cancellationToken);

                case PartitioningMode.Parallel:
                    return ExecuteBatchesParallelAsync(batches, cancellationToken);

                default:
                    string message = string.Format(Resources.TableQueryExecutorInvalidPartitioningMode, _configuration.PartitioningMode);
                    throw new InvalidOperationException(message);
            }
        }

        private IList<TEntity> ExecuteBatchesSequentially(IEnumerable<TableBatchOperation> batches)
        {
            var result = new List<TEntity>();

            foreach (TableBatchOperation batch in batches)
            {
                IEnumerable<TEntity> entities = _cloudTable
                    .ExecuteBatch(batch)
                    .Select(p => _entityConverter.GetEntity((DynamicTableEntity) p.Result));

                result.AddRange(entities);
            }

            return result;
        }

        private Task<IList<TEntity>> ExecuteBatchesSequentiallyAsync(IEnumerable<TableBatchOperation> batches, CancellationToken cancellationToken)
        {
            var queue = new ConcurrentQueue<TEntity>();

            IEnumerable<Task> tasks = batches.Select(
                p => _cloudTable
                         .ExecuteBatchAsync(p, cancellationToken)
                         .Then(results =>
                             {
                                 foreach (TableResult tableResult in results)
                                 {
                                     queue.Enqueue(_entityConverter.GetEntity((DynamicTableEntity) tableResult.Result));
                                 }
                             }));

            return TaskHelpers.Iterate(tasks, cancellationToken)
                              .ContinueWith(p => (IList<TEntity>) queue.ToList(), cancellationToken);
        }

        private IList<TEntity> ExecuteBatchesParallel(IEnumerable<TableBatchOperation> batches)
        {
            var queue = new ConcurrentQueue<TEntity>();

            Parallel.ForEach(batches, batch =>
                {
                    IEnumerable<TEntity> entities = _cloudTable
                        .ExecuteBatch(batch)
                        .Select(p => _entityConverter.GetEntity((DynamicTableEntity) p.Result));

                    foreach (TEntity entity in entities)
                    {
                        queue.Enqueue(entity);
                    }
                });

            return queue.ToList();
        }

        private Task<IList<TEntity>> ExecuteBatchesParallelAsync(IEnumerable<TableBatchOperation> batches, CancellationToken cancellationToken)
        {
            var queue = new ConcurrentQueue<TEntity>();

            IEnumerable<Task> tasks = batches.Select(
                p => _cloudTable
                         .ExecuteBatchAsync(p, cancellationToken)
                         .Then(results =>
                             {
                                 foreach (TableResult tableResult in results)
                                 {
                                     queue.Enqueue(_entityConverter.GetEntity((DynamicTableEntity) tableResult.Result));
                                 }
                             }, cancellationToken));

            return Task.Factory.ContinueWhenAll(
                tasks.ToArray(),
                results => (IList<TEntity>) queue.ToList(),
                cancellationToken);
        }
    }
}