using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Tests.Attributes;
using WindowsAzure.Tests.Common;
using Xunit;

namespace WindowsAzure.Tests.Table.Extensions
{
    public sealed class CloudTableClientExtensionsIntegrationTests : TestBase, IDisposable
    {
        private readonly CloudTableClient _tableClient;
        private readonly List<string> _tableNames;

        public CloudTableClientExtensionsIntegrationTests()
        {
            _tableClient = GenerateCloudTableClient();
            _tableNames = new List<string>();

            for (int i = 0; i < 3; i++)
            {
                string tableName = string.Format("T{0}", Guid.NewGuid().ToString("N"));

                _tableNames.Add(tableName);
                CloudTable cloudTable = _tableClient.GetTableReference(tableName);
                cloudTable.CreateIfNotExists();
            }
        }

        public void Dispose()
        {
            foreach (string tableName in _tableNames)
            {
                CloudTable cloudTable = _tableClient.GetTableReference(tableName);
                cloudTable.DeleteIfExists();
            }
        }

        // Only for real Azure Storage
        //[IntegrationalFact]
        //public async Task CloudTableClientChangeServiceVersionTest()
        //{
        //    // Arrange
        //    const string serviceVersion = "2012-02-12";

        //    // Act
        //    ServiceProperties properties = await _tableClient.GetServicePropertiesAsync();
        //    properties.DefaultServiceVersion = serviceVersion;
        //    await _tableClient.SetServicePropertiesAsync(properties);
        //    ServiceProperties result = await _tableClient.GetServicePropertiesAsync();

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.Equal(properties.DefaultServiceVersion, serviceVersion);
        //}

        [IntegrationFact]
        public async Task CloudTableClientGetAllTablesTest()
        {
            // Act
            List<CloudTable> allTables = await _tableClient.ListTablesAsync();

            // Assert
            Assert.NotNull(allTables);
            Assert.True(_tableNames.All(s => allTables.Select(p => p.Name).Contains(s)));
        }

        [IntegrationFact]
        public async Task CloudTableClientGetTablesWithTPrefixTest()
        {
            // Arrange
            const string prefix = "T";

            // Act
            List<CloudTable> tableWithTPrefix = await _tableClient.ListTablesAsync(prefix);

            // Assert
            Assert.NotNull(tableWithTPrefix);
            Assert.True(_tableNames.All(p => p.StartsWith(prefix)));
        }

        [IntegrationFact]
        public async Task CloudTableClientGetTwoTablesWithTPrefixTest()
        {
            // Arrange
            const string prefix = "T";
            const int count = 2;

            // Act
            List<CloudTable> twoTablesWithTPrefix = await _tableClient.ListTablesAsync(prefix, count);

            // Assert
            Assert.NotNull(twoTablesWithTPrefix);
            Assert.Equal(twoTablesWithTPrefix.Count, count);
            Assert.True(_tableNames.All(p => p.StartsWith(prefix)));
        }
    }
}