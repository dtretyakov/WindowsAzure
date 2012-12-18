using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.Infrastructure;
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
            var country = new Country {Continent = "Europe"};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData.PartitionKey.GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.String);
            Assert.Equal(entityProperty.StringValue, country.Continent);
        }

        [Fact]
        public void GetBinaryEntityPropertyTest()
        {
            // Arrange
            var country = new Country {TopSecretKey = new byte[] {0xfa, 0x55, 0x22}};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "TopSecretKey")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Binary);
            Assert.Equal(entityProperty.BinaryValue, country.TopSecretKey);
        }

        [Fact]
        public void GetBooleanEntityPropertyTest()
        {
            // Arrange
            var country = new Country {IsExists = true};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "IsExists")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Boolean);
            Assert.Equal(entityProperty.BooleanValue, country.IsExists);
        }

        [Fact]
        public void GetDateTimeEntityPropertyTest()
        {
            // Arrange
            var country = new Country {Formed = new DateTime(1999, 5, 7)};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Formed")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.DateTime);
            Assert.True(entityProperty.DateTimeOffsetValue.HasValue);
// ReSharper disable PossibleInvalidOperationException
            Assert.Equal(entityProperty.DateTimeOffsetValue.Value.DateTime, country.Formed);
// ReSharper restore PossibleInvalidOperationException
        }

        [Fact]
        public void GetDoubleEntityPropertyTest()
        {
            // Arrange
            var country = new Country {Area = 333.334};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Area")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Double);
            Assert.Equal(entityProperty.DoubleValue, country.Area);
        }

        [Fact]
        public void GetGuidEntityPropertyTest()
        {
            // Arrange
            var country = new Country {Id = Guid.NewGuid()};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Id")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Guid);
            Assert.Equal(entityProperty.GuidValue, country.Id);
        }

        [Fact]
        public void GetInt32EntityPropertyTest()
        {
            // Arrange
            var country = new Country {PresidentsCount = 34};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "PresidentsCount")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Int32);
            Assert.Equal(entityProperty.Int32Value, country.PresidentsCount);
        }

        [Fact]
        public void GetInt64EntityPropertyTest()
        {
            // Arrange
            var country = new Country {Population = 2446878754356789347};
            var entityTypeData = new EntityTypeData(typeof (Country));

            // Act
            EntityProperty entityProperty = entityTypeData
                .Properties
                .Single(p => p.Name == "Population")
                .GetEntityProperty(country);

            // Assert
            Assert.NotNull(entityProperty);
            Assert.Equal(entityProperty.PropertyType, EdmType.Int64);
            Assert.Equal(entityProperty.Int64Value, country.Population);
        }
    }
}