using System;
using System.Threading.Tasks;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable
{
    public sealed class TableQueryProviderTests
    {
        [Fact]
        public void CreateTableQueryProviderWithNullCloudTable()
        {
            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableQueryProvider<Country>(null, null));
        }

        [Fact]
        public void CreateTableQueryProviderWithNullEntityConverter()
        {
            // Arrange
            var cloudTableMock = MocksFactory.GetCloudTableMock();

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => new TableQueryProvider<Country>(cloudTableMock.Object, null));
        }

        [Fact]
        public void ExecuteWithNullExpression()
        {
            // Arrange
            var cloudTableMock = MocksFactory.GetCloudTableMock();
            var converter = new TableEntityConverter<Country>();
            var provider = new TableQueryProvider<Country>(cloudTableMock.Object, converter);

            // Act && Assert
            Assert.Throws<ArgumentNullException>(() => provider.Execute(null));
        }

        [Fact]
        public Task ExecuteAsyncWithNullExpression()
        {
            // Arrange
            var cloudTableMock = MocksFactory.GetCloudTableMock();
            var converter = new TableEntityConverter<Country>();
            var provider = new TableQueryProvider<Country>(cloudTableMock.Object, converter);

            // Act && Assert
            return Assert.ThrowsAsync<ArgumentNullException>(() => provider.ExecuteAsync(null));
        }
    }
}