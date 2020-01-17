using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public class EntityWithSerializeAttribute
    {
        [Serialize(Name =  "NestedSerialized")]
        public NestedEntity Nested { get; set; }

        [PartitionKey]
        public string Pk { get; set; }

        [RowKey]
        public string Rk { get; set; }

        public class NestedEntity
        {
            public decimal DecimalValue { get; set; }
        }
    }
}    
