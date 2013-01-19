using System;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;

namespace WindowsAzure.Tests.Table
{
    public class LogEntryTableSetBase : TestBase, IDisposable
    {
        protected readonly string TableName;

        protected LogEntryTableSetBase()
        {
            CloudTableClient tableClient = GenerateCloudTableClient();
            TableName = "Table" + Guid.NewGuid().ToString("N");

            tableClient.GetTableReference(TableName).CreateIfNotExists();
        }

        public void Dispose()
        {
            CloudTableClient tableClient = GenerateCloudTableClient();

            tableClient.GetTableReference(TableName).DeleteIfExists();
        }

        /// <summary>
        ///     Creates a new tableset.
        /// </summary>
        /// <returns>TableSet context.</returns>
        protected TableSet<LogEntry> GetTableSet()
        {
            CloudTableClient tableClient = GenerateCloudTableClient();

            return new TableSet<LogEntry>(tableClient, TableName);
        }
    }
}