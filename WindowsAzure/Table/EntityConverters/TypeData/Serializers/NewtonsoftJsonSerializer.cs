using Newtonsoft.Json;

namespace WindowsAzure.Table.EntityConverters.TypeData.Serializers
{
    public sealed class NewtonsoftJsonSerializer : ISerializer
    {
        public string Serialize(object value) => JsonConvert.SerializeObject(value);     
        
        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);
    }
}
