using System;
using GitHub.WindowsAzure.Table;

namespace GitHub.WindowsAzure.Tests.Table
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
    }
}