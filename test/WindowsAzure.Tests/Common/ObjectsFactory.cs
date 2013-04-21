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
    }
}