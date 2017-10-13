using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class EntityWithContructor
    {
        public EntityWithContructor(string value)
        {
            Property = value;
        }

        public string Property { get; set; }

        [PartitionKey]
        public string PartitionKey { get; set; }
    }
}