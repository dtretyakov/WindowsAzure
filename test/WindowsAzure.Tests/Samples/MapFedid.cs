using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public class MapFedid
    {
        [PartitionKey]
        public string Context { get; set; }

        [RowKey]
        public string FederatedId { get; set; }
    }
}