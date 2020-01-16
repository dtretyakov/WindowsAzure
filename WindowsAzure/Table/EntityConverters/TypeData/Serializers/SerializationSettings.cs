using System;

namespace WindowsAzure.Table.EntityConverters.TypeData.Serializers
{
    public sealed class SerializationSettings
    {
        private static readonly Lazy<SerializationSettings> instance = new Lazy<SerializationSettings>(
            () => new SerializationSettings
            {
                Default = new NewtonsoftJsonSerializer(),
            });

        private SerializationSettings()
        {
        }

        public static SerializationSettings Instance
        {
            get
            {
                return instance.Value;
            }
        }

        public bool SerializeComplexTypes { get; set; }

        public ISerializer Default { get; set; }
    }
}
