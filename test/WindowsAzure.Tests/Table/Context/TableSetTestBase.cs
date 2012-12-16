using System;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;

namespace WindowsAzure.Tests.Table.Context
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