using System;
using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Tests.Samples
{
    public sealed class LogEntry
    {
        [PartitionKey] public string Id;

        [Property(Name = "OldMessage")] public string Message;

        [Timestamp] public DateTime Timestamp;

        [ETag]
        public string ETag { get; set; }

        [Ignore]
        public byte[] PrivateData;
    }
}