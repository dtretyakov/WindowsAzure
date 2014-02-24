using System;
using WindowsAzure.Table.EntityConverters.TypeData;

namespace WindowsAzure.Tests.Samples
{
    public class AddressMapping : EntityTypeMap<Address>
    {
        public AddressMapping()
        {
            PartitionKey(e => e.Country);
            RowKey(e => e.Street);
        }
    }

    public class AddressInvalidMapping : EntityTypeMap<AddressInvalidMap>
    {
        public AddressInvalidMapping()
        {
            PartitionKey(e => e.Country);
            RowKey(e => e.Id);
        }
    }

    public sealed class AddressInvalidMap : Address
    {
    }

    public class Address
    {
        public string Country { get; set; }

        public string District { get; set; }

        public long State { get; set; }

        public string Street { get; set; }

        public double Area { get; set; }

        public DateTime Formed { get; set; }

        public Guid Id { get; set; }

        public int PresidentsCount { get; set; }

        public byte[] TopSecretKey { get; set; }

        public bool IsExists { get; set; }
    }
}
