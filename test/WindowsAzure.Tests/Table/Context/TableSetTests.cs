using System;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Context
{
    public sealed class TableSetTests
    {
        [Fact]
        public void CreateTableSetWithTableClientParameter()
        {
            // Arrange
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();

            // Act
            var context = new TableSet<Country>(tableClient);

            // Assert
            Assert.Equal(typeof (Country).Name, context.CloudTable.Name);
        }

        [Fact]
        public void CreateTableSetWithNullTableClientParameter()
        {
            TableSet<Country> context = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => { context = new TableSet<Country>(null); });

            Assert.Null(context);
        }

        [Fact]
        public void CreateTableSetWithTableClientAndNameParameters()
        {
            // Arrange
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            const string tableName = "tableName";

            // Act
            var context = new TableSet<Country>(tableClient, tableName);

            // Assert
            Assert.Equal(tableName, context.CloudTable.Name);
        }

        [Fact]
        public void CreateTableSetWithNullTableNameParameter()
        {
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            TableSet<Country> context = null;

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => { context = new TableSet<Country>(tableClient, null); });

            Assert.Null(context);
        }
    }
}