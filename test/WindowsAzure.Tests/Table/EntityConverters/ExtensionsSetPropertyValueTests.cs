using System;
using System.Linq;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.Infrastructure;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class ExtensionsSetPropertyValueTests
    {
        [Fact]
        public void SetStringPropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            const string value = "Poland";

            // Act
            entityTypeData.RowKey.SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.Name, value);
        }

        [Fact]
        public void SetBinaryPropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            var value = new byte[] {0x33, 0x55, 0x77};

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "TopSecretKey")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.TopSecretKey, value);
        }

        [Fact]
        public void SetBooleanPropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            const bool value = true;

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "IsExists")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.IsExists, value);
        }

        [Fact]
        public void SetDateTimePropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            DateTime value = DateTime.UtcNow;

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "Formed")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.Formed, value);
        }

        [Fact]
        public void SetDoublePropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            const double value = 435435435.546678;

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "Area")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.Area, value);
        }

        [Fact]
        public void SetGuidPropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            Guid value = Guid.NewGuid();

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "Id")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.Id, value);
        }

        [Fact]
        public void SetInt32PropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            const int value = 345678;

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "PresidentsCount")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.PresidentsCount, value);
        }

        [Fact]
        public void SetInt64PropertyValueTest()
        {
            // Arrange
            var country = new Country();
            var entityTypeData = new EntityTypeData<Country>();
            const long value = 3456783456789;

            // Act
            entityTypeData
                .Properties
                .Single(p => p.Name == "Population")
                .SetValue(country, new EntityProperty(value).GetValue());

            // Assert
            Assert.Equal(country.Population, value);
        }
    }
}