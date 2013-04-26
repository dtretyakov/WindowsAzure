using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Tests.Samples
{
    public sealed class CountryTableEntity : TableEntity
    {
        public long Population { get; set; }

        public double Area { get; set; }

        public DateTime Formed { get; set; }

        public Guid Id { get; set; }

        public int PresidentsCount { get; set; }

        public byte[] TopSecretKey { get; set; }

        public bool IsExists { get; set; }
    }
}