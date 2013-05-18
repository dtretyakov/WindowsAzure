using System;
using WindowsAzure.Table.Wrappers;
using Xunit;

namespace WindowsAzure.Tests.Wrappers
{
    public sealed class WrappersTests
    {
        [Fact]
        public void CreateCloudTableWrapperWithNullArgument()
        {
            // Arrange
            CloudTableWrapper result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => result = new CloudTableWrapper(null));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void CreateTableQueryWrapperWithNullArgument()
        {
            // Arrange
            TableQueryWrapper result = null;

            // Act
            Assert.Throws<ArgumentNullException>(() => result = new TableQueryWrapper(null));

            // Assert
            Assert.Null(result);
        }
    }
}