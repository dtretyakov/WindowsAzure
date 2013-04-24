using System;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.QueryExecutor;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.QueryExecutor
{
    public sealed class ParallelExecutorTests
    {
        [Fact]
        public void CreateExecutorWithNullCloudTableParameter()
        {
            // Arrange
            Mock<ITableEntityConverter<Country>> entityConverterMock = MocksFactory.GetTableEntityConverterMock<Country>();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableQueryParallelExecutor<Country>(null, entityConverterMock.Object));
        }

        [Fact]
        public void CreateExecutorWithNullEntityConverterParameter()
        {
            // Arrange
            CloudTable cloudTable = ObjectsFactory.GetCloudTable();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableQueryParallelExecutor<Country>(cloudTable, null));
        }

        // TODO: Wrap CloudTable or wait for resolving https://github.com/WindowsAzure/azure-sdk-for-net/issues/202
    }
}