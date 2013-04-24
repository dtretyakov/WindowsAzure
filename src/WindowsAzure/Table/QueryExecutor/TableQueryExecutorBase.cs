using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;

namespace WindowsAzure.Table.QueryExecutor
{
    internal abstract class TableQueryExecutorBase<T> : ITableQueryExecutor<T> where T : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        internal TableQueryExecutorBase(CloudTable cloudTable, ITableEntityConverter<T> entityConverter)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException("cloudTable");
            }

            if (entityConverter == null)
            {
                throw new ArgumentNullException("entityConverter");
            }

            _cloudTable = cloudTable;
            _entityConverter = entityConverter;
        }

        /// <summary>
        ///     Executes operation.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <returns>Result entity.</returns>
        public T Execute(T entity, Func<ITableEntity, TableOperation> operation)
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
        public Task<T> ExecuteAsync(T entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken)
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

        public abstract IEnumerable<T> ExecuteBatches(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation);

        public abstract Task<IEnumerable<T>> ExecuteBatchesAsync(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);
    }
}