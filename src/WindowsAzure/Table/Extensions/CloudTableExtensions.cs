using System.Collections.Generic;
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
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>Task.</returns>
        public static Task CreateAsync(this CloudTable cloudTable,
                                       TableRequestOptions requestOptions = null,
                                       OperationContext operationContext = null,
                                       object state = null)
        {
            return Task.Factory.FromAsync(cloudTable.BeginCreate, cloudTable.EndCreate,
                                          requestOptions, operationContext, state);
        }

        /// <summary>
        ///     Begins an asynchronous operation to delete a table.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>
        ///     <c>true</c> if table was created; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> CreateIfNotExistsAsync(this CloudTable cloudTable,
                                                        TableRequestOptions requestOptions = null,
                                                        OperationContext operationContext = null,
                                                        object state = null)
        {
            return Task<bool>.Factory.FromAsync(cloudTable.BeginCreateIfNotExists, cloudTable.EndCreateIfNotExists,
                                                requestOptions, operationContext, state);
        }

        /// <summary>
        ///     Begins an asynchronous operation to delete a table.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>Task.</returns>
        public static Task DeleteAsync(this CloudTable cloudTable,
                                       TableRequestOptions requestOptions = null,
                                       OperationContext operationContext = null,
                                       object state = null)
        {
            return Task.Factory.FromAsync(cloudTable.BeginDelete, cloudTable.EndDelete,
                                          requestOptions, operationContext, state);
        }

        /// <summary>
        ///     Begins an asynchronous operation to delete the tables if it exists.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>
        ///     <c>true</c> if the table was deleted; otherwise, <c>false</c>.
        /// </returns>
        public static Task<bool> DeleteIfNotExistsAsync(this CloudTable cloudTable,
                                                        TableRequestOptions requestOptions = null,
                                                        OperationContext operationContext = null,
                                                        object state = null)
        {
            return Task<bool>.Factory.FromAsync(cloudTable.BeginDeleteIfExists, cloudTable.EndDeleteIfExists,
                                                requestOptions, operationContext, state);
        }

        /// <summary>
        ///     Begins an asynchronous table operation using the specified
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" />
        ///     and
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" />.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="operation">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableOperation" /> object that represents the operation to perform.
        /// </param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> containing the result of executing the operation on the table.
        /// </returns>
        public static Task<TableResult> ExecuteAsync(this CloudTable cloudTable,
                                                     TableOperation operation,
                                                     TableRequestOptions requestOptions = null,
                                                     OperationContext operationContext = null,
                                                     object state = null)
        {
            return Task<TableResult>.Factory.FromAsync(cloudTable.BeginExecute, cloudTable.EndExecute,
                                                       operation, requestOptions, operationContext, state);
        }

        /// <summary>
        ///     Executes a query on a table, using the specified
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" />
        ///     and
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.OperationContext" />
        ///     .
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="query">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> representing the query to execute.
        /// </param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        public static async Task<IEnumerable<DynamicTableEntity>> ExecuteQueryAsync(
            this CloudTable cloudTable,
            TableQuery query,
            TableRequestOptions requestOptions = null,
            OperationContext operationContext = null)
        {
            var result = new List<DynamicTableEntity>();
            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<DynamicTableEntity> entities =
                    await cloudTable.ExecuteQuerySegmentedAsync(query, token, requestOptions, operationContext);

                result.AddRange(entities.Results);

                token = entities.ContinuationToken;
            } while (token != null && query.TakeCount.HasValue && query.TakeCount.Value < result.Count);

            return result;
        }

        /// <summary>
        ///     Begins an asynchronous operation to execute a batch of operations on a table, using the specified
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" />
        ///     and
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" />.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="batch">
        ///     The <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" /> object representing the operations to execute on the table.
        /// </param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> objects that contains the results, in order, of each operation in the
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" />
        ///     on the table.
        /// </returns>
        public static Task<IList<TableResult>> ExecuteBatchAsync(this CloudTable cloudTable,
                                                                 TableBatchOperation batch,
                                                                 TableRequestOptions requestOptions = null,
                                                                 OperationContext operationContext = null,
                                                                 object state = null)
        {
            return Task<IList<TableResult>>.Factory.FromAsync(cloudTable.BeginExecuteBatch, cloudTable.EndExecuteBatch,
                                                              batch, requestOptions, operationContext, state);
        }

        /// <summary>
        ///     Begins an asynchronous operation to query a table in segmented mode, using the specified
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" />
        ///     and
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" />
        ///     continuation token.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="query">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> instance specifying the table to query and the query parameters to use, specialized for a type <c>TElement</c>.
        /// </param>
        /// <param name="token">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" /> object representing a continuation token from the server when the operation returns a partial result.
        /// </param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// *
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuerySegment`1" /> containing the projection into type <c>R</c> of the results of executing the query.
        /// </returns>
        public static Task<TableQuerySegment<DynamicTableEntity>> ExecuteQuerySegmentedAsync(
            this CloudTable cloudTable,
            TableQuery query,
            TableContinuationToken token,
            TableRequestOptions requestOptions = null,
            OperationContext operationContext = null,
            object state = null)
        {
            ICancellableAsyncResult cancellableAsyncResult = cloudTable.BeginExecuteQuerySegmented(
                query, token, requestOptions, operationContext, null, state);

            return Task<TableQuerySegment<DynamicTableEntity>>.Factory.FromAsync(
                cancellableAsyncResult,
                cloudTable.EndExecuteQuerySegmented);
        }

        /// <summary>
        ///     Begins an asynchronous operation to query a table in segmented mode, using the specified
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" />
        ///     and
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" />
        ///     continuation token.
        /// </summary>
        /// <typeparam name="TElement">The entity type of the query.</typeparam>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="query">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> instance specifying the table to query and the query parameters to use, specialized for a type <c>TElement</c>.
        /// </param>
        /// <param name="token">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" /> object representing a continuation token from the server when the operation returns a partial result.
        /// </param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// *
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuerySegment`1" /> containing the projection into type <c>R</c> of the results of executing the query.
        /// </returns>
        public static Task<TableQuerySegment<TElement>> ExecuteQuerySegmentedAsync<TElement>(
            this CloudTable cloudTable,
            TableQuery<TElement> query,
            TableContinuationToken token,
            TableRequestOptions requestOptions = null,
            OperationContext operationContext = null,
            object state = null) where TElement : ITableEntity, new()
        {
            ICancellableAsyncResult cancellableAsyncResult = cloudTable.BeginExecuteQuerySegmented(
                query, token, requestOptions, operationContext, null, state);

            return Task<TableQuerySegment<TElement>>.Factory.FromAsync(
                cancellableAsyncResult,
                cloudTable.EndExecuteQuerySegmented<TElement>);
        }

        /// <summary>
        ///     Begins an asynchronous operation to query a table in segmented mode, using the specified
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" />
        ///     and
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" />
        ///     continuation token.
        /// </summary>
        /// <typeparam name="TElement">The entity type of the query.</typeparam>
        /// <typeparam name="TR">
        ///     The type into which the <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" /> will project the query results.
        /// </typeparam>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="query">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuery" /> instance specifying the table to query and the query parameters to use, specialized for a type <c>TElement</c>.
        /// </param>
        /// <param name="resolver">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.Table.EntityResolver" /> instance which creates a projection of the table query result entities into the specified type <c>R</c>.
        /// </param>
        /// <param name="token">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableContinuationToken" /> object representing a continuation token from the server when the operation returns a partial result.
        /// </param>
        /// <param name="requestOptions">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableRequestOptions" /> object that specifies execution options, such as retry policy and timeout settings, for the operation.
        /// </param>
        /// <param name="operationContext">
        ///     An <see cref="T:Microsoft.WindowsAzure.Storage.OperationContext" /> object for tracking the current operation.
        /// </param>
        /// <param name="state">A user-defined object that will be passed to the callback delegate.</param>
        /// *
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableQuerySegment`1" /> containing the projection into type <c>R</c> of the results of executing the query.
        /// </returns>
        public static Task<TableQuerySegment<TR>> ExecuteQuerySegmentedAsync<TElement, TR>(
            this CloudTable cloudTable,
            TableQuery<TElement> query,
            EntityResolver<TR> resolver,
            TableContinuationToken token,
            TableRequestOptions requestOptions = null,
            OperationContext operationContext = null,
            object state = null) where TElement : ITableEntity, new()
        {
            ICancellableAsyncResult cancellableAsyncResult = cloudTable.BeginExecuteQuerySegmented(
                query, resolver, token, requestOptions, operationContext, null, state);

            return Task<TableQuerySegment<TR>>.Factory.FromAsync(
                cancellableAsyncResult,
                cloudTable.EndExecuteQuerySegmented<TElement, TR>);
        }
    }
}