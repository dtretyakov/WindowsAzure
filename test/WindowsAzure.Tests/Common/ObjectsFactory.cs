using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Tests.Samples;

namespace WindowsAzure.Tests.Common
{
    public static class ObjectsFactory
    {
        public static CloudTableClient GetCloudTableClient()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            return storageAccount.CreateCloudTableClient();
        }

        public static DynamicTableEntity GetCountryDynamicTableEntity()
        {
            return new DynamicTableEntity
            {
                
                PartitionKey = "Europe",
                RowKey = "Spain",
                Properties = new Dictionary<string, EntityProperty>()
                    {
                        {"Area", new EntityProperty(505992.0)},
                        {"TopSecretKey", new EntityProperty(new byte[] { 0xaa, 0xbb, 0xcc })},
                        {"Formed", new EntityProperty(new DateTime(1812, 1, 1))},
                        {"Id", new EntityProperty(Guid.NewGuid())},
                        {"IsExists", new EntityProperty(true)},
                        {"Population", new EntityProperty(47190493L)},
                        {"PresidentsCount", new EntityProperty(8)}
                    }
            };
        }

        public static Country GetCountry()
        {
            return new Country
                {
                    Area = 505992,
                    Continent = "Europe",
                    TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                    Formed = new DateTime(1812, 1, 1),
                    Id = Guid.NewGuid(),
                    IsExists = true,
                    Name = "Spain",
                    Population = 47190493,
                    PresidentsCount = 8
                };
        }

        public static CountryTableEntity GetTableCountry()
        {
            return new CountryTableEntity
            {
                Area = 505992,
                PartitionKey = "Europe",
                TopSecretKey = new byte[] { 0xaa, 0xbb, 0xcc },
                Formed = new DateTime(1812, 1, 1),
                Id = Guid.NewGuid(),
                IsExists = true,
                RowKey = "Spain",
                Population = 47190493,
                PresidentsCount = 8
            };
        }

        public static IList<Country> GetCountries()
        {
            return new List<Country>
                {
                    new Country
                        {
                            Area = 505992,
                            Continent = "Europe",
                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                            Formed = new DateTime(1812, 1, 1),
                            Id = Guid.NewGuid(),
                            IsExists = true,
                            Name = "Spain",
                            Population = 47190493,
                            PresidentsCount = 8
                        },
                    new Country
                        {
                            Area = 5059922,
                            Continent = "Europe",
                            TopSecretKey = new byte[] {0xaa, 0xbb, 0xcc},
                            Formed = new DateTime(1813, 1, 1),
                            Id = Guid.NewGuid(),
                            IsExists = false,
                            Name = "Spain2",
                            Population = 471904932,
                            PresidentsCount = 82
                        }
                };
        }

        public static CloudTable GetCloudTable()
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient();
            return tableClient.GetTableReference("Table");
        }
    }
}