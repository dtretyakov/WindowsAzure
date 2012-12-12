using System.Linq;
using GitHub.WindowsAzure.Table;
using GitHub.WindowsAzure.Table.EntityFormatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Tests.Table
{
    [TestClass]
    public sealed class TableSetQueryTests
    {
        private TableSet<Country> _tableSet;

        [TestInitialize]
        public void Initialize()
        {
            CloudStorageAccount cloudStorageAccount = //CloudStorageAccount.DevelopmentStorageAccount;
                CloudStorageAccount.Parse("UseDevelopmentStorage=true;DevelopmentStorageProxyUri=http://ipv4.fiddler");
            CloudTableClient cloudTableClient = cloudStorageAccount.CreateCloudTableClient();

            _tableSet = new TableSet<Country>(cloudTableClient, new TableEntityFormatter<Country>());
        }

        [TestMethod]
        public void FirstQuery()
        {
            const int value = 123;

            var query = _tableSet.Where(p => p.Area > 300).Take(1);

            var values = query.ToList();
        }
    }
}