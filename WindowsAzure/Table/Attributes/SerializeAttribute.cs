using System;

namespace WindowsAzure.Table.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SerializeAttribute : Attribute
    {
        public string Name { get; set; }

        public Type SerializerType { get; }

        public SerializeAttribute()
        {
        }

        public SerializeAttribute(string name)
        {
            Name = name;
        }        

        public SerializeAttribute(Type serializerType, string targetName = null)
            : this(targetName)
        {
            SerializerType = serializerType;
        }        
    }
}
