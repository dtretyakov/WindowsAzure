namespace WindowsAzure.Table.EntityConverters.TypeData.Serializers
{
    public interface ISerializer
    {
        string Serialize(object value);

        T Deserialize<T>(string value);
    }
}
