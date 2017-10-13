using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.RequestExecutor
{
    /// <summary>
    ///     Handles query execution.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface ITableRequestExecutor<T> where T : new()
    {
        /// <summary>
        ///     Executes operation.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <returns>Result entity.</returns>
        T Execute(T entity, Func<ITableEntity, TableOperation> operation);

        /// <summary>
        ///     Executes operation without returning result.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        void ExecuteWithoutResult(T entity, Func<ITableEntity, TableOperation> operation);

        /// <summary>
        ///     Executes operation asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entity.</returns>
        Task<T> ExecuteAsync(T entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);

        /// <summary>
        ///     Executes operation without returning result asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="operation">Operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteWithoutResultAsync(T entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);

        /// <summary>
        ///     Executes batch operations.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Result entities.</returns>
        IEnumerable<T> ExecuteBatches(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation);

        /// <summary>
        ///     Executes batch operations without returning results.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        void ExecuteBatchesWithoutResult(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation);

        /// <summary>
        ///     Executes batch operations asynchronously.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result entities.</returns>
        Task<IEnumerable<T>> ExecuteBatchesAsync(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);

        /// <summary>
        ///     Executes batch operations without returning results asynchronously.
        /// </summary>
        /// <param name="entities">List of entities.</param>
        /// <param name="operation">Table operation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteBatchesWithoutResultAsync(IEnumerable<T> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken);
    }
}