using System;

namespace WindowsAzure.Table.EntityConverters.TypeData.Serializers
{
    /// <summary>
    /// Defines serialization settings 
    /// </summary>
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

        /// <summary>
        /// SerializationSettings Instance 
        /// </summary>
        public static SerializationSettings Instance
        {
            get
            {
                return instance.Value;
            }
        }

        /// <summary>
        /// Serialize non supported types by default
        /// </summary>
        public bool SerializeComplexTypes { get; set; }


        /// <summary>
        /// Default serializer
        /// </summary>
        public ISerializer Default { get; set; }
    }
}
