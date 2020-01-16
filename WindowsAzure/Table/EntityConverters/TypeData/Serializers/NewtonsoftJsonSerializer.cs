using Newtonsoft.Json;

namespace WindowsAzure.Table.EntityConverters.TypeData.Serializers
{
    public class NewtonsoftJsonSerializer : ISerializer
    {
        public T Deserialize<T>(string value) => JsonConvert.DeserializeObject<T>(value);      

        public string Serialize(object value) => JsonConvert.SerializeObject(value);        
    }
}
