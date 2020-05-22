using System;
using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public class EntityWithMultipleUnsuportedTypes
    {
        [PartitionKey]
        public string Pk { get; set; }

        [RowKey]
        public string Rk { get; set; }

        public decimal DecimalValue { get; set; }

        public Nested NestedValue { get; set; }

        public class Nested
        {
            public TimeSpan TimeSpanValue { get; set; }
        }
    }
}
