using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.Infrastructure;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class ExtensionsGetEntityPropertyTests
    {
        [Fact]
        public void GetStringEntityPropertyTest()
        {
            // Arrange
            var entityTypeData = new EntityTypeData<Country>();
            const string value = "Europe";

            // Act
            EntityProperty entityProperty = entityTypeData.PartitionKey.Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.String);
            Assert.Equal(entityProperty.StringValue, value);
        }

        [Fact]
        public void GetBinaryEntityPropertyTest()
        {
            // Arrange
            var value = new byte[] {0xfa, 0x55, 0x22};
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "TopSecretKey")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Binary);
            Assert.Equal(entityProperty.BinaryValue, value);
        }

        [Fact]
        public void GetBooleanEntityPropertyTest()
        {
            // Arrange
            const bool value = true;
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "IsExists")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Boolean);
            Assert.Equal(entityProperty.BooleanValue, value);
        }

        [Fact]
        public void GetDateTimeEntityPropertyTest()
        {
            // Arrange
            var value = new DateTime(1999, 5, 7);
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Formed")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.DateTime);
            Assert.True(entityProperty.DateTimeOffsetValue.HasValue);
// ReSharper disable PossibleInvalidOperationException
            Assert.Equal(entityProperty.DateTimeOffsetValue.Value.DateTime, value);
// ReSharper restore PossibleInvalidOperationException
        }

        [Fact]
        public void GetDoubleEntityPropertyTest()
        {
            // Arrange
            const double value = 333.334;
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Area")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Double);
            Assert.Equal(entityProperty.DoubleValue, value);
        }

        [Fact]
        public void GetGuidEntityPropertyTest()
        {
            // Arrange
            var value = Guid.NewGuid();
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Id")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Guid);
            Assert.Equal(entityProperty.GuidValue, value);
        }

        [Fact]
        public void GetInt32EntityPropertyTest()
        {
            // Arrange
            const int value = 34;
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "PresidentsCount")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Int32);
            Assert.Equal(entityProperty.Int32Value, value);
        }

        [Fact]
        public void GetInt64EntityPropertyTest()
        {
            // Arrange
            const long value = 2446878754356789347;
            var entityTypeData = new EntityTypeData<Country>();

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Population")
                .Type.GetEntityProperty(value);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Int64);
            Assert.Equal(entityProperty.Int64Value, value);
        }
    }
}