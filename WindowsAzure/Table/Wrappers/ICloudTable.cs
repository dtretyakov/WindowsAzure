using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Wrappers
{
    /// <summary>
    ///     Public interface of <see cref="CloudTable" />.
    /// </summary>
    internal interface ICloudTable
    {
        /// <summary>
        ///     Gets the table name.
        /// </summary>
        /// <value> The table name. </value>
        string Name { get; }

        /// <summary>
        ///     Executes a query on a table.
        /// </summary>
        /// <param name="tableQuery">
        ///     A <see cref="ITableQuery" /> representing the query to execute.
        /// </param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        IEnumerable<DynamicTableEntity> ExecuteQuery(ITableQuery tableQuery);

        /// <summary>
        ///     Executes a query on a table asynchronously.
        /// </summary>
        /// <param name="tableQuery">Table query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        Task<IEnumerable<DynamicTableEntity>> ExecuteQueryAsync(ITableQuery tableQuery, CancellationToken cancellationToken);

        /// <summary>
        ///     Executes the operation on a table.
        /// </summary>
        /// <param name="operation">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableOperation" /> object that represents the operation to perform.
        /// </param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> containing the result of executing the operation on the table.
        /// </returns>
        TableResult Execute(TableOperation operation);

        /// <summary>
        ///     Executes table operation asynchronously.
        /// </summary>
        /// <param name="operation">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableOperation" /> object that represents the operation to perform.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> containing the result of executing the operation on the table.
        /// </returns>
        Task<TableResult> ExecuteAsync(TableOperation operation, CancellationToken cancellationToken);

        /// <summary>
        ///     Executes a batch operation on a table as an atomic operation.
        /// </summary>
        /// <param name="batch">
        ///     The <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" /> object representing the operations to execute on the table.
        /// </param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> objects that contains the results, in order, of each operation in the
        ///     <see
        ///         cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" />
        ///     on the table.
        /// </returns>
        IList<TableResult> ExecuteBatch(TableBatchOperation batch);

        /// <summary>
        ///     Executes a batch of operations on a table asynchronously.
        /// </summary>
        /// <param name="tableBatchOperation">
        ///     The <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" /> object representing the operations to execute on the table.
        /// </param>
        /// <param name="cancellationToken">Cancalltion token.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> objects that contains the results, in order, of each operation in the
        ///     <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableBatchOperation" />
        ///     on the table.
        /// </returns>
        Task<IList<TableResult>> ExecuteBatchAsync(TableBatchOperation tableBatchOperation, CancellationToken cancellationToken);
    }
}