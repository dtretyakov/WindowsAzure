using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Extensions;

namespace WindowsAzure.Table.Wrappers
{
    /// <summary>
    ///     Wrapper around <see cref="CloudTable" />.
    /// </summary>
    internal sealed class CloudTableWrapper : ICloudTable
    {
        private readonly CloudTable _cloudTable;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        public CloudTableWrapper(CloudTable cloudTable)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException("cloudTable");
            }

            _cloudTable = cloudTable;
        }

        /// <summary>
        ///     Executes a query on a table.
        /// </summary>
        /// <param name="tableQuery">
        ///     A <see cref="ITableQuery" /> representing the query to execute.
        /// </param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        public IEnumerable<DynamicTableEntity> ExecuteQuery(ITableQuery tableQuery)
        {
            var query = new TableQuery
                {
                    FilterString = tableQuery.FilterString,
                    SelectColumns = tableQuery.SelectColumns,
                    TakeCount = tableQuery.TakeCount
                };

            return _cloudTable.ExecuteQuery(query);
        }

        /// <summary>
        ///     Executes a query on a table asynchronously.
        /// </summary>
        /// <param name="tableQuery">Table query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of <see cref="T:Microsoft.WindowsAzure.Storage.Table.DynamicTableEntity" /> objects, representing table entities returned by the query.
        /// </returns>
        public Task<IEnumerable<DynamicTableEntity>> ExecuteQueryAsync(ITableQuery tableQuery, CancellationToken cancellationToken)
        {
            var query = new TableQuery
                {
                    FilterString = tableQuery.FilterString,
                    SelectColumns = tableQuery.SelectColumns,
                    TakeCount = tableQuery.TakeCount
                };

            return _cloudTable.ExecuteQueryAsync(query, cancellationToken);
        }

        /// <summary>
        ///     Executes the operation on a table.
        /// </summary>
        /// <param name="operation">
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableOperation" /> object that represents the operation to perform.
        /// </param>
        /// <returns>
        ///     A <see cref="T:Microsoft.WindowsAzure.Storage.Table.TableResult" /> containing the result of executing the operation on the table.
        /// </returns>
        public TableResult Execute(TableOperation operation)
        {
            return _cloudTable.Execute(operation);
        }

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
        public Task<TableResult> ExecuteAsync(TableOperation operation, CancellationToken cancellationToken)
        {
            return _cloudTable.ExecuteAsync(operation, cancellationToken);
        }

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
        public IList<TableResult> ExecuteBatch(TableBatchOperation batch)
        {
            return _cloudTable.ExecuteBatch(batch);
        }

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
        public Task<IList<TableResult>> ExecuteBatchAsync(TableBatchOperation tableBatchOperation, CancellationToken cancellationToken)
        {
            return _cloudTable.ExecuteBatchAsync(tableBatchOperation, cancellationToken);
        }

        /// <summary>
        ///     Gets the table name.
        /// </summary>
        /// <value> The table name. </value>
        public string Name
        {
            get { return _cloudTable.Name; }
        }
    }
}