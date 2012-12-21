using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Extensions
{
    /// <summary>
    ///     Cloud Table Extensions.
    /// </summary>
    public static class CloudTableExtensions
    {
        /// <summary>
        ///     Begins an asynchronous operation to create a table.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task CreateAsync(
            this CloudTable cloudTable,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginCreate(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudTable.EndCreate(result);
                    });
        }

        /// <summary>
        ///     Begins an asynchronous operation to delete a table.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if table was created; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> CreateIfNotExistsAsync(
            this CloudTable cloudTable,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginCreateIfNotExists(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndCreateIfNotExists(result);
                    });
        }

        /// <summary>
        ///     Begins an asynchronous operation to delete a table.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task.</returns>
        public static Task DeleteAsync(
            this CloudTable cloudTable,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginDelete(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudTable.EndDelete(result);
                    });
        }

        /// <summary>
        ///     Begins an asynchronous operation to delete the tables if it exists.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the table was deleted; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> DeleteIfNotExistsAsync(
            this CloudTable cloudTable,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginDeleteIfExists(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndDeleteIfExists(result);
                    });
        }

        /// <summary>
        ///     Executes table operation asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableOperation">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableOperation" /> object that represents the operation to perform.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> containing the result of executing the operation on the table.
        /// </returns>
        public static Task<TableResult> ExecuteAsync(
            this CloudTable cloudTable,
            TableOperation tableOperation,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginExecute(tableOperation, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TableResult>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndExecute(result);
                    });
        }

        /// <summary>
        ///     Executes a query on a table asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableQuery">Table query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        public static Task<IEnumerable<DynamicTableEntity>> ExecuteQueryAsync(
            this CloudTable cloudTable,
            TableQuery tableQuery,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteQuerySegmentedImplAsync(
                cloudTable, new List<DynamicTableEntity>(), tableQuery, null, cancellationToken)
                .Then(results => (IEnumerable<DynamicTableEntity>) results);
        }

        /// <summary>
        ///     Aggregates query execution segments.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableEntities">Table entities.</param>
        /// <param name="tableQuery">Table query.</param>
        /// <param name="continuationToken">Continuation token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        private static Task<List<DynamicTableEntity>> ExecuteQuerySegmentedImplAsync(
            this CloudTable cloudTable,
            List<DynamicTableEntity> tableEntities,
            TableQuery tableQuery,
            TableContinuationToken continuationToken,
            CancellationToken cancellationToken)
        {
            return cloudTable
                .ExecuteQuerySegmentedAsync(tableQuery, continuationToken, cancellationToken)
                .Then(result =>
                          {
                              tableEntities.AddRange(result.Results);

                              TableContinuationToken continuation = result.ContinuationToken;

                              // Checks whether enumeration has completed
                              if (continuation != null && tableQuery.TakeCount.HasValue &&
                                  tableQuery.TakeCount.Value < tableEntities.Count)
                              {
                                  cancellationToken.ThrowIfCancellationRequested();

                                  return ExecuteQuerySegmentedImplAsync(
                                      cloudTable, tableEntities, tableQuery, continuation, cancellationToken);
                              }

                              return TaskHelpers.FromResult(tableEntities);
                          });
        }

        /// <summary>
        ///     Executes a batch of operations on a table asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableBatchOperation">
        ///     The <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" /> object representing the operations to execute on the table.
        /// </param>
        /// <param name="cancellationToken">Cancalltion token.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> objects that contains the results, in order, of each operation in the
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" />
        ///     on the table.
        /// </returns>
        public static Task<IList<TableResult>> ExecuteBatchAsync(
            this CloudTable cloudTable,
            TableBatchOperation tableBatchOperation,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginExecuteBatch(tableBatchOperation, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<IList<TableResult>>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndExecuteBatch(result);
                    });
        }

        /// <summary>
        ///     Executes an operation to query a table in segmented mode asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableQuery">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> instance specifying the table to query and the query parameters to use, specialized for a type <c>TElement</c>.
        /// </param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" /> object representing a continuation token from the server when the operation returns a partial result.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuerySegment`1" /> containing the projection into type <c>R</c> of the results of executing the query.
        /// </returns>
        public static Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
            this CloudTable cloudTable,
            TableQuery tableQuery,
            TableContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginExecuteQuerySegmented(
                tableQuery, continuationToken, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TableQuerySegment<DynamicTableEntity>>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndExecuteQuerySegmented(result);
                    });
        }

        /// <summary>
        ///     Executes an operation to query a table in segmented mode asynchronously.
        /// </summary>
        /// <typeparam name="TElement">The entity type of the query.</typeparam>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableQuery">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> instance specifying the table to query and the query parameters to use, specialized for a type <c>TElement</c>.
        /// </param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" /> object representing a continuation token from the server when the operation returns a partial result.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuerySegment`1" /> containing the projection into type <c>R</c> of the results of executing the query.
        /// </returns>
        public static Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedAsync<TElement>(
            this CloudTable cloudTable,
            TableQuery<TElement> tableQuery,
            TableContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
            where TElement : ITableEntity, new()
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginExecuteQuerySegmented(
                tableQuery, continuationToken, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TableQuerySegment<TElement>>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndExecuteQuerySegmented<TElement>(result);
                    });
        }

        /// <summary>
        ///     Executes an operation to query a table in segmented mode asynchronously.
        /// </summary>
        /// <typeparam name="TElement">The entity type of the query.</typeparam>
        /// <typeparam name="TR">
        ///     The type into which the <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" /> will project the query results.
        /// </typeparam>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tableQuery">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> instance specifying the table to query and the query parameters to use, specialized for a type <c>TElement</c>.
        /// </param>
        /// <param name="entityResolver">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" /> instance which creates a projection of the table query result entities into the specified type <c>R</c>.
        /// </param>
        /// <param name="continuationToken">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" /> object representing a continuation token from the server when the operation returns a partial result.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuerySegment`1" /> containing the projection into type <c>R</c> of the results of executing the query.
        /// </returns>
        public static Task<TableQuerySegment<TR>> ExecuteQuerySegmentedAsync<TElement, TR>(
            this CloudTable cloudTable,
            TableQuery<TElement> tableQuery,
            EntityResolver<TR> entityResolver,
            TableContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
            where TElement : ITableEntity, new()
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginExecuteQuerySegmented(
                tableQuery, entityResolver, continuationToken, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TableQuerySegment<TR>>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndExecuteQuerySegmented<TElement, TR>(result);
                    });
        }
    }
}