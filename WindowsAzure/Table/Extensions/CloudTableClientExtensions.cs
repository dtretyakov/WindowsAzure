using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#elif AZUREDATATABLES
using Azure.Data.Tables;
using DynamicTableEntity = Azure.Data.Tables.TableEntity;
#else
using Microsoft.Azure.Cosmos.Table;
#endif

namespace WindowsAzure.Table.Extensions
{
    /// <summary>
    ///     Cloud table client extensions.
    /// </summary>
    public static class CloudTableClientExtensions
    {

        /// <summary>
        ///     Gets the properties of the table service asynchronously.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An enumerable collection of tables that are retrieved.</returns>
        public static Task<List<CloudTable>> ListTablesAsync(
            this CloudTableClient tableClient,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListTablesImplAsync(tableClient, new List<CloudTable>(), null, null, null, cancellationToken);
        }

        /// <summary>
        ///     Gets the properties of the table service asynchronously.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="prefix">The table name prefix.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An enumerable collection of tables that are retrieved.</returns>
        public static Task<List<CloudTable>> ListTablesAsync(
            this CloudTableClient tableClient,
            string prefix,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListTablesImplAsync(tableClient, new List<CloudTable>(), prefix, null, null, cancellationToken);
        }

        /// <summary>
        ///     Gets the properties of the table service asynchronously.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="prefix">The table name prefix.</param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is zero the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An enumerable collection of tables that are retrieved.</returns>
        public static Task<List<CloudTable>> ListTablesAsync(
            this CloudTableClient tableClient,
            string prefix,
            int? maxResults,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return ListTablesImplAsync(tableClient, new List<CloudTable>(), prefix, maxResults, null, cancellationToken);
        }


        /// <summary>
        ///     Returns an enumerable collection of tables in the storage account asynchronously.
        /// </summary>
        /// <param name="tableClient">Cloud table client.</param>
        /// <param name="cloudTables">List of cloud tables.</param>
        /// <param name="prefix">The table name prefix.</param>
        /// <param name="maxResults">
        ///     A non-negative integer value that indicates the maximum number of results to be returned at a time, up to the
        ///     per-operation limit of 5000. If this value is zero the maximum possible number of results will be returned, up to 5000.
        /// </param>
        /// <param name="continuationToken">Continuation token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>
        ///     An enumerable collection of tables that are retrieved.
        /// </returns>
        private static Task<List<CloudTable>> ListTablesImplAsync(
            this CloudTableClient tableClient,
            List<CloudTable> cloudTables,
            string prefix,
            int? maxResults,
            TableContinuationToken continuationToken,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            return tableClient
                .ListTablesSegmentedAsync(prefix, maxResults, continuationToken, null, null, cancellationToken)
                .Then(result =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();

                        cloudTables.AddRange(result.Results);

                        // Checks whether maxresults entities has been received
                        if (maxResults.HasValue && cloudTables.Count >= maxResults.Value)
                        {
                            return TaskHelpers.FromResult(cloudTables.Take(maxResults.Value).ToList());
                        }

                        // Checks whether enumeration has been completed
                        if (result.ContinuationToken != null)
                        {
                            return ListTablesImplAsync(tableClient, cloudTables, prefix, maxResults, result.ContinuationToken, cancellationToken);
                        }

                        return TaskHelpers.FromResult(cloudTables);
                    });
        }

#if !NET45
        /// <summary>
        ///     Creates the table if it does not already exist.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if table was created; otherwise, <c>false</c>.
        /// </returns>
        public static bool CreateIfNotExists(this CloudTable table)
        {
            return table.CreateIfNotExistsAsync().ExecuteSynchronously();
        }

        public static bool DeleteIfExists(this CloudTable table)
        {
            return table.DeleteIfExistsAsync().ExecuteSynchronously();
        }
#endif
    }
}