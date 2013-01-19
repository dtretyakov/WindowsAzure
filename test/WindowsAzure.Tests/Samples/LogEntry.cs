using System;
using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class LogEntry
    {
        [PartitionKey] public string Id;
        public string Message;

        [Timestamp] public DateTime Timestamp;

        [ETag]
        public string ETag { get; set; }
    }
}