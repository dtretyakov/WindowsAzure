using System;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Tests.Common
{
    public class TestBase
    {
        private const string StorageConfigurationName = "storage.xml";

        static TestBase()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            XDocument xDocument;

            try
            {
                xDocument = XDocument.Load(StorageConfigurationName);
            }
            catch (Exception)
            {
                xDocument = null;
            }

            if (xDocument != null)
            {
                XElement xName = xDocument.XPathSelectElement("/account/name");
                XElement xKey = xDocument.XPathSelectElement("/account/key");

                if (xName != null && !string.IsNullOrEmpty(xName.Value) &&
                    xKey != null && !string.IsNullOrEmpty(xKey.Value))
                {
                    storageAccount = new CloudStorageAccount(new StorageCredentials(xName.Value, xKey.Value), false);
                }
            }

            StorageCredentials = storageAccount.Credentials;

            QueueServiceEndpoint = storageAccount.QueueEndpoint;
            TableServiceEndpoint = storageAccount.TableEndpoint;
            BlobServiceEndpoint = storageAccount.BlobEndpoint;
        }

        public static Uri QueueServiceEndpoint { get; set; }

        public static Uri TableServiceEndpoint { get; set; }

        public static Uri BlobServiceEndpoint { get; set; }

        /// <summary>
        ///     The StorageCredentials created from account settings in the target tenant config.
        /// </summary>
        public static StorageCredentials StorageCredentials { get; private set; }

        public static CloudTableClient GenerateCloudTableClient()
        {
            return new CloudTableClient(TableServiceEndpoint, StorageCredentials);
        }

        public static CloudBlobClient GenerateCloudBlobClient()
        {
            return new CloudBlobClient(BlobServiceEndpoint, StorageCredentials);
        }

        public static CloudQueueClient GenerateCloudQueueClient()
        {
            return new CloudQueueClient(QueueServiceEndpoint, StorageCredentials);
        }
    }
}