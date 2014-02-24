using System;
using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class Country
    {
        [PartitionKey]
        public string Continent { get; set; }

        [RowKey]
        public string Name { get; set; }

        public long Population { get; set; }

        public double Area { get; set; }

        public DateTime Formed { get; set; }

        public Guid Id { get; set; }

        public int PresidentsCount { get; set; }

        public byte[] TopSecretKey { get; set; }

        public bool IsExists { get; set; }
    }
}