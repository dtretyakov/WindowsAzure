using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class EntityWithInvalidPropertyAttribute
    {
        [PartitionKey]
        public string Id { get; set; }

        [Property("My name")]
        public string Property { get; set; }
    }
}