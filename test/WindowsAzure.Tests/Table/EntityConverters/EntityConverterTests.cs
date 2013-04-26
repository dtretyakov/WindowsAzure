using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Tests.Common;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.EntityConverters
{
    public sealed class EntityConverterTests
    {
        private const string ResultFormat = "{0} iterations took {1} ms";
        private const int IteractionsCount = 50000000;

        // ReSharper disable PossibleInvalidOperationException

        [Fact(Skip = "Only for performance measurement")]
        public void DynamicToObjectMeasurement()
        {
            // Arrange
            DynamicTableEntity dynamicTableEntity = ObjectsFactory.GetCountryDynamicTableEntity();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Country country = null;

            // Act
            for (int i = 0; i < IteractionsCount; i++)
            {
                country = new Country
                    {
                        Area = (double) dynamicTableEntity.Properties["Area"].DoubleValue,
                        Continent = dynamicTableEntity.PartitionKey,
                        Formed = dynamicTableEntity.Properties["Formed"].DateTimeOffsetValue.Value.DateTime,
                        Id = (Guid) dynamicTableEntity.Properties["Id"].GuidValue,
                        IsExists = (bool) dynamicTableEntity.Properties["IsExists"].BooleanValue,
                        Name = dynamicTableEntity.RowKey,
                        Population = (long) dynamicTableEntity.Properties["Population"].Int64Value,
                        PresidentsCount = (int) dynamicTableEntity["PresidentsCount"].Int32Value,
                        TopSecretKey = dynamicTableEntity["TopSecretKey"].BinaryValue
                    };
            }

            stopWatch.Stop();

            Assert.NotNull(country);

            Console.WriteLine(ResultFormat, IteractionsCount, stopWatch.ElapsedMilliseconds);
        }

        // ReSharper restore PossibleInvalidOperationException

        [Fact(Skip = "Only for performance measurement")]
        public void ObjectToDynamicMeasurement()
        {
            // Arrange
            Country entity = ObjectsFactory.GetCountry();
            Stopwatch stopWatch = Stopwatch.StartNew();
            DynamicTableEntity dynamicTableEntity = null;

            // Act
            for (int i = 0; i < IteractionsCount; i++)
            {
                dynamicTableEntity = new DynamicTableEntity(entity.Continent, entity.Name)
                    {
                        Properties = new Dictionary<string, EntityProperty>
                            {
                                {"Area", new EntityProperty(entity.Area)},
                                {"TopSecretKey", new EntityProperty(entity.TopSecretKey)},
                                {"Formed", new EntityProperty(entity.Formed)},
                                {"Id", new EntityProperty(entity.Id)},
                                {"IsExists", new EntityProperty(entity.IsExists)},
                                {"Population", new EntityProperty(entity.Population)},
                                {"PresidentsCount", new EntityProperty(entity.PresidentsCount)}
                            }
                    };
            }

            stopWatch.Stop();

            Assert.NotNull(dynamicTableEntity);

            Console.WriteLine(ResultFormat, IteractionsCount, stopWatch.ElapsedMilliseconds);
        }

        [Fact(Skip = "Only for performance measurement")]
        public void DynamicToPocoMeasurement()
        {
            // Arrange
            var converter = new TableEntityConverter<Country>();
            DynamicTableEntity dynamicTableEntity = ObjectsFactory.GetCountryDynamicTableEntity();
            Stopwatch stopWatch = Stopwatch.StartNew();
            Country entity = null;

            // Act
            for (int i = 0; i < IteractionsCount; i++)
            {
                entity = converter.GetEntity(dynamicTableEntity);
            }

            stopWatch.Stop();

            Assert.NotNull(entity);

            Console.WriteLine(ResultFormat, IteractionsCount, stopWatch.ElapsedMilliseconds);
        }

        [Fact(Skip = "Only for performance measurement")]
        public void PocoToDynamicMeasurement()
        {
            // Arrange
            var converter = new TableEntityConverter<Country>();
            Country entity = ObjectsFactory.GetCountry();
            Stopwatch stopWatch = Stopwatch.StartNew();
            ITableEntity tableEntity = null;

            // Act
            for (int i = 0; i < IteractionsCount; i++)
            {
                tableEntity = converter.GetEntity(entity);
            }

            stopWatch.Stop();

            Assert.NotNull(tableEntity);

            Console.WriteLine(ResultFormat, IteractionsCount, stopWatch.ElapsedMilliseconds);
        }

        [Fact(Skip = "Only for performance measurement")]
        public void DynamicToTableMeasurement()
        {
            // Arrange
            DynamicTableEntity dynamicTableEntity = ObjectsFactory.GetCountryDynamicTableEntity();
            Stopwatch stopWatch = Stopwatch.StartNew();
            var tableEntity = new CountryTableEntity();

            // Act
            for (int i = 0; i < IteractionsCount; i++)
            {
                tableEntity.PartitionKey = dynamicTableEntity.PartitionKey;
                tableEntity.RowKey = dynamicTableEntity.RowKey;
                tableEntity.ReadEntity(dynamicTableEntity.Properties, null);
            }

            stopWatch.Stop();

            Assert.NotNull(tableEntity);

            Console.WriteLine(ResultFormat, IteractionsCount, stopWatch.ElapsedMilliseconds);
        }

        [Fact(Skip = "Only for performance measurement")]
        public void TableToDynamicMeasurement()
        {
            // Arrange
            CountryTableEntity entity = ObjectsFactory.GetTableCountry();
            Stopwatch stopWatch = Stopwatch.StartNew();
            var dynamicTableEntity = new DynamicTableEntity();

            // Act
            for (int i = 0; i < IteractionsCount; i++)
            {
                dynamicTableEntity.PartitionKey = entity.PartitionKey;
                dynamicTableEntity.RowKey = entity.RowKey;
                dynamicTableEntity.Properties = entity.WriteEntity(null);
            }

            stopWatch.Stop();

            Assert.NotNull(dynamicTableEntity);

            Console.WriteLine(ResultFormat, IteractionsCount, stopWatch.ElapsedMilliseconds);
        }
    }
}