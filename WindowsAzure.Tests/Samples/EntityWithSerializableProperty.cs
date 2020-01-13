using WindowsAzure.Table.EntityConverters.TypeData;

namespace WindowsAzure.Tests.Samples
{
    public class EntityWithSerializableProperty
    {
        public string Pk { get; set; }
        public string Rk { get; set; }
        public SerializableEntity SerializableEntity { get; set; }
    }

    public class SerializableEntity
    {
        public int IntValue { get; set; }
    }

    public class EntityWithSerializablePropertyMapping : EntityTypeMap<EntityWithSerializableProperty>
    {
        public EntityWithSerializablePropertyMapping()
        {
            PartitionKey(x=> x.Pk)
            .RowKey(x=> x.Rk)
            .JsonSerialize(x => x.SerializableEntity, "NestedEntityRaw");
        }
    }
}
