using System;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Samples;
using Microsoft.WindowsAzure.Storage.Table;
using Xunit;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;

namespace WindowsAzure.Tests.Table.EntityConverters.TypeData
{
    public sealed class EntityTypeMapTests 
    {      

        [Fact]
        public void RegisterClassMap_RowKeyIsNotString_ExceptionThrown()
        {
            EntityTypeMap<Address> map = null;

            // Arrange & Act & Asset
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                map = new EntityTypeMap<Address>(e => 
                    e.PartitionKey(p => p.Country).RowKey(p => p.Id));
            });

            Assert.Null(map);
        }

        [Fact]
        public void RegisterClassMap_PartitionKeyIsNotString_ExceptionThrown()
        {
            EntityTypeMap<Address> map = null;

            // Arrange & Act & Asset
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                map = new EntityTypeMap<Address>(e =>
                    e.PartitionKey(p => p.Id).RowKey(p => p.Country));
            });

            Assert.Null(map);
        }

        [Fact]
        public void RegisterClassMap_KeysAreValid()
        {
            // Arrange & Act
            var map = new EntityTypeMap<Address>(e => 
                e.PartitionKey(p => p.Country).RowKey(p => p.Street));

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

            var entity = (DynamicTableEntity) map.GetEntity(new Address());
            Assert.Equal(8, entity.Properties.Count);
        }

        [Fact]
        public void GetEntityTypeData_IgnorePropertyMap()
        {
            // Arrange & Act
            var map = EntityTypeDataFactory.GetEntityTypeData<AddressIgnoreProperties>();

            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.Equal(2, map.NameChanges.Count);
            Assert.Equal("PartitionKey", map.NameChanges["Country"]);
            Assert.Equal("RowKey", map.NameChanges["Street"]);

            var entity = (DynamicTableEntity)map.GetEntity(new AddressIgnoreProperties());
            Assert.Equal(8, entity.Properties.Count);
        }

        [Fact]
        public void GetEntityTypeData_InvalidPropertyMap()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
            {
                var map = EntityTypeDataFactory.GetEntityTypeData<AddressInvalidProperty>();
            });
        }

        [Fact]
        public void GetEntityTypeData_IgnoreProperty()
        {
            // Arrange & Act
            var map = new EntityTypeMap<Address>(e => 
                e.PartitionKey(p => p.Country)
                .RowKey(p => p.Street)
                .Ignore(p => p.Area));

            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.Equal(2, map.NameChanges.Count);
            Assert.Equal("PartitionKey", map.NameChanges["Country"]);
            Assert.Equal("RowKey", map.NameChanges["Street"]);

            var entity = (DynamicTableEntity) map.GetEntity(new Address());
            Assert.Equal(7, entity.Properties.Count);
        }

        [Fact]
        public void GetEntityTypeData_InvalidMap()
        {
            // Arrange & Act & Asset
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                EntityTypeDataFactory.GetEntityTypeData<AddressInvalidMap>();
            });
        }

        [Fact]
        public void CreateTypeMapWithOnlyOneKey()
        {
            // Arrange & Act
            var map = new EntityTypeMap<Address>(e => e.PartitionKey(p => p.Country));

            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.Equal(1, map.NameChanges.Count);
            Assert.Equal("PartitionKey", map.NameChanges["Country"]);
        }

        [Fact]
        public void GetEntityTypeData_IgnoreProperty_EvenWhenUnsupportedType()
        {
            // Arrange & Act
            var map = new EntityTypeMap<EntityWithInvalidPropertyType>(e =>
                e.PartitionKey(p => p.PKey)
                .RowKey(p => p.RKey)
                .Ignore(p => p.Country));

            // Assert
            Assert.NotNull(map.NameChanges);
            Assert.DoesNotContain(map.NameChanges, t => t.Key == "Country");
            Assert.Equal(2, map.NameChanges.Count);
        }

        [Fact]
        public void ConvertFromEntityWithSerializeMapping()
        {
            // Arrange
            var serializer = SerializationSettings.Instance.Default;

            var map = new EntityTypeMap<EntityWithSerializableProperty>(e =>
                 e.PartitionKey(x => x.Pk)
                .RowKey(x => x.Rk)
                .Serialize(x => x.SerializableEntity, "NestedEntityRaw")
            );

            var entity = new EntityWithSerializableProperty
            {
                SerializableEntity = new SerializableEntity
                {
                    DecimalValue = 4,
                },
            };

            // Act
            var tableEntity = (DynamicTableEntity)map.GetEntity(entity);

            // Assert
            Assert.NotNull(tableEntity.Properties["NestedEntityRaw"].StringValue);

            var deserialized = serializer.Deserialize<SerializableEntity>(tableEntity.Properties["NestedEntityRaw"].StringValue);
            Assert.Equal(entity.SerializableEntity.DecimalValue, deserialized.DecimalValue);
        }
    }
}