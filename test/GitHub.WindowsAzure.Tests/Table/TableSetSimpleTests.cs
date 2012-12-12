using System;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table;
using GitHub.WindowsAzure.Table.EntityFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Tests.Table
{
    [TestClass]
    public sealed class TableSetSimpleTests
    {
        private TableSet<Country> _tableSet;

        [TestInitialize]
        public void Initialize()
        {
            var cloudStorageAccount = //CloudStorageAccount.DevelopmentStorageAccount;
                CloudStorageAccount.Parse("UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://ipv4.fiddler");
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            _tableSet = new TableSet<Country>(cloudTableClient, new TableEntityFormatter<Country>());
        }

        [TestMethod]
        public async Task AddNewEntityTest()
        {
            var country = new Country
                              {
                                  Continent = "Asia",
                                  Area = 1222.4,
                                  Formed = new DateTime(1991, 12, 21),
                                  Name = "Russia2",
                                  Population = 140000000
                              };

            await _tableSet.AddAsync(country);
        }

        [TestMethod]
        public async Task UpdateEntityTest()
        {
            var country = new Country
                              {
                                  Area = 324,
                                  Continent = "Asia",
                                  Name = "Russia",
                                  Formed = new DateTime(324324324324),
                                  Population = 23432434
                              };

            await _tableSet.UpdateAsync(country);
        }
    }
}