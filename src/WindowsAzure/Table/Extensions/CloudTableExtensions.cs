using System.Collections.Generic;
using System.Linq;
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
        ///     Creates a table asynchronously.
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
        ///     Creates the table if it does not already exist asynchronously.
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
        ///     Deletes a table asynchronously.
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
        ///     Deletes the table if it exists asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if the table was deleted; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> DeleteIfExistsAsync(
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
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return cloudTable
                .ExecuteQuerySegmentedAsync(tableQuery, continuationToken, cancellationToken)
                .Then(result =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        tableEntities.AddRange(result.Results);

                        // Checks whether TakeCount entities has been received
                        if (tableQuery.TakeCount.HasValue && tableEntities.Count >= tableQuery.TakeCount.Value)
                        {
                            return TaskHelpers.FromResult(tableEntities.Take(tableQuery.TakeCount.Value).ToList());
                        }

                        // Checks whether enumeration has been completed
                        if (result.ContinuationToken != null)
                        {
                            return ExecuteQuerySegmentedImplAsync(cloudTable, tableEntities, tableQuery, result.ContinuationToken, cancellationToken);
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

        /// <summary>
        ///     Checks whether the table exists asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     <c>true</c> if table exists; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> ExistsAsync(
            this CloudTable cloudTable,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginExists(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<bool>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndExists(result);
                    });
        }

        /// <summary>
        ///     Gets the permissions settings for the table asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     The table's permissions.
        /// </returns>
        public static Task<TablePermissions> GetPermissionsAsync(
            this CloudTable cloudTable,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginGetPermissions(null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task<TablePermissions>.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        return cloudTable.EndGetPermissions(result);
                    });
        }

        /// <summary>
        ///     Sets the permissions settings for the table asynchronously.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="tablePermissions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TablePermissions" /> object that represents the permissions to set.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task SetPermissionsAsync(
            this CloudTable cloudTable,
            TablePermissions tablePermissions,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            ICancellableAsyncResult asyncResult = cloudTable.BeginSetPermissions(tablePermissions, null, null);
            CancellationTokenRegistration registration = cancellationToken.Register(p => asyncResult.Cancel(), null);

            return Task.Factory.FromAsync(
                asyncResult,
                result =>
                    {
                        registration.Dispose();
                        cloudTable.EndSetPermissions(result);
                    });
        }
    }
}