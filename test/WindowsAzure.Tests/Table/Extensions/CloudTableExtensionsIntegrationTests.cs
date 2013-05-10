using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Tests.Attributes;
using WindowsAzure.Tests.Common;
using Xunit;

namespace WindowsAzure.Tests.Table.Extensions
{
    public sealed class CloudTableExtensionsIntegrationTests : TestBase, IDisposable
    {
        private readonly CloudTable _cloudTable;

        public CloudTableExtensionsIntegrationTests()
        {
            string tableName = "T" + Guid.NewGuid().ToString("N");
            CloudTableClient tableClient = GenerateCloudTableClient();
            _cloudTable = tableClient.GetTableReference(tableName);
        }

        public void Dispose()
        {
            _cloudTable.DeleteIfExists();
        }

        [IntegrationFact]
        public async Task CloudTableExtensionsTest()
        {
            // Arrange
            var permissions = new TablePermissions();
            permissions.SharedAccessPolicies.Add("default", new SharedAccessTablePolicy
                {
                    Permissions = SharedAccessTablePermissions.Query,
                    SharedAccessExpiryTime = DateTimeOffset.UtcNow,
                    SharedAccessStartTime = DateTimeOffset.UtcNow
                });

            // Act

            // Create & delete table
            await _cloudTable.CreateAsync();
            await _cloudTable.DeleteAsync();

            // Create table
            bool createIfNotExistsResult = await _cloudTable.CreateIfNotExistsAsync();

            // Check whether table exists
            bool existsResult = await _cloudTable.ExistsAsync();

            // Set & get permissions
            await _cloudTable.SetPermissionsAsync(permissions);
            TablePermissions permissionsResult = await _cloudTable.GetPermissionsAsync();

            // Delete table
            bool deleteIfExistsResult = await _cloudTable.DeleteIfExistsAsync();

            // Assert
            Assert.True(createIfNotExistsResult);
            Assert.True(existsResult);
            Assert.NotNull(permissionsResult);
            Assert.NotNull(permissionsResult.SharedAccessPolicies);
            Assert.Equal(permissionsResult.SharedAccessPolicies.Count, permissions.SharedAccessPolicies.Count);

            foreach (var policy in permissionsResult.SharedAccessPolicies)
            {
                Assert.Contains(policy.Key, permissions.SharedAccessPolicies.Keys);
                SharedAccessTablePolicy value = permissions.SharedAccessPolicies[policy.Key];
                Assert.Equal(policy.Value.Permissions, value.Permissions);
            }

            Assert.True(deleteIfExistsResult);
        }
    }
}