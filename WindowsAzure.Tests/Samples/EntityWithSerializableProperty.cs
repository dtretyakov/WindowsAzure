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
        public decimal DecimalValue { get; set; }
    }    
}