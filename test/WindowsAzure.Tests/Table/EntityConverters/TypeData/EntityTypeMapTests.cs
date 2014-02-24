
using System;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class EntityTypeMapTests
    {
        [Fact]
        public void RegisterClassMap_RowKeyIsNotString_ExceptionThrown()
        {
            // Arrange & Act & Asset
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var map = new EntityTypeMap<Address>(e => e.PartitionKey(p => p.Country)
                                                                  .RowKey(p => p.Id));
            });
        }

        [Fact]
        public void RegisterClassMap_PartitionKeyIsNotString_ExceptionThrown()
        {
            // Arrange & Act & Asset
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var map = new EntityTypeMap<Address>(e => e.PartitionKey(p => p.Id)
                                                                  .RowKey(p => p.Country));
            });
        }

        [Fact]
        public void RegisterClassMap_KeysAreValid()
        {
            // Arrange & Act
            var map = new EntityTypeMap<Address>(e => e.PartitionKey(p => p.Country)
                                                              .RowKey(p => p.Street));

            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.Equal(2, map.NameChanges.Count);
            Assert.Equal("PartitionKey", map.NameChanges["Country"]);
            Assert.Equal("RowKey", map.NameChanges["Street"]);
        }

        [Fact]
        public void GetEntityTypeData_ValidMap()
        {
            // Arrange & Act
            var map = EntityTypeDataFactory.GetEntityTypeData<Address>();

            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.Equal(2, map.NameChanges.Count);
            Assert.Equal("PartitionKey", map.NameChanges["Country"]);
            Assert.Equal("RowKey", map.NameChanges["Street"]);
        }

        [Fact]
        public void GetEntityTypeData_InvalidMap()
        {
            // Arrange & Act & Asset
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                var map = EntityTypeDataFactory.GetEntityTypeData<AddressInvalidMap>();
            });
        }
    }
}