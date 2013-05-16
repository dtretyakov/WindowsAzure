using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class EntityWithMultipleAttributes
    {
        [RowKey]
        [PartitionKey]
        public string ID { get; set; }
    }
}