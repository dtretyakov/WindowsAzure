using System;
using GitHub.WindowsAzure.Table;
using GitHub.WindowsAzure.Tests.Common;
using GitHub.WindowsAzure.Tests.Samples;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Tests.Table.Context
{
    public class TableSetTestBase : TestBase, IDisposable
    {
        private readonly string _tableName;

        protected TableSetTestBase()
        {
            CloudTableClient tableClient = GenerateCloudTableClient();
            _tableName = "Table" + Guid.NewGuid().ToString("N");

            tableClient.GetTableReference(_tableName).CreateIfNotExists();
        }

        public void Dispose()
        {
            CloudTableClient tableClient = GenerateCloudTableClient();

            tableClient.GetTableReference(_tableName).DeleteIfExists();
        }

        /// <summary>
        ///     Creates a new tableset.
        /// </summary>
        /// <returns>TableSet context.</returns>
        protected TableSet<Country> GetTableSet()
        {
            CloudTableClient tableClient = GenerateCloudTableClient();

            return new TableSet<Country>(tableClient, _tableName);
        }
    }
}