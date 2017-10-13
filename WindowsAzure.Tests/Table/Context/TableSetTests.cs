using System;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table;
using WindowsAzure.Table.RequestExecutor;
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
            Assert.Equal(typeof (Country).Name, context.RequestExecutorFactory.CloudTable.Name);
            Assert.Equal(ExecutionMode.Sequential, context.ExecutionMode);
            Assert.IsType<TableRequestSequentialExecutor<Country>>(context.RequestExecutor);
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
            Assert.Equal(tableName, context.RequestExecutorFactory.CloudTable.Name);
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

        [Fact]
        public void SetTheSameExecutionMode()
        {
            // Arrange
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient);
            var executor = context.RequestExecutor;

            // Act
            context.ExecutionMode = ExecutionMode.Sequential;

            // Assert
            Assert.Equal(ExecutionMode.Sequential, context.ExecutionMode);
            Assert.IsType<TableRequestSequentialExecutor<Country>>(context.RequestExecutor);
            Assert.Same(executor, context.RequestExecutor);
        }

        [Fact]
        public void ChangeTableSetExecutionMode()
        {
            // Arrange
            CloudTableClient tableClient = ObjectsFactory.GetCloudTableClient();
            var context = new TableSet<Country>(tableClient);

            // Act
            context.ExecutionMode = ExecutionMode.Parallel;

            // Assert
            Assert.Equal(ExecutionMode.Parallel, context.ExecutionMode);
            Assert.IsType<TableRequestParallelExecutor<Country>>(context.RequestExecutor);
        }
    }
}