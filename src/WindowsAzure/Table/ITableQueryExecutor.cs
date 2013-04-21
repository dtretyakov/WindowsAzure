using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table
{
    public interface ITableQueryExecutor<TEntity> where TEntity : new()
    {
        /// <summary>
        ///     Executes operation.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <returns>Result entity.</returns>
        TEntity Execute(TEntity entity, Func<ITableEntity, TableOperation> operation);

        /// <summary>
        ///     Executes operation asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entity.</returns>
        Task<TEntity> ExecuteAsync(TEntity entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);

        /// <summary>
        ///     Executes batch operations.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Result entities.</returns>
        IList<TEntity> ExecuteBatches(IList<TEntity> entities, Func<ITableEntity, TableOperation> operation);

        /// <summary>
        ///     Executes batch operations asynchronously.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entities.</returns>
        Task<IList<TEntity>> ExecuteBatchesAsync(IList<TEntity> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);
    }
}