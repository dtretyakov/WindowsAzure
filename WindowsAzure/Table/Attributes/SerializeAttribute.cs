using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    /// Defines whether property should be serialized
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SerializeAttribute : Attribute
    {
        /// <summary>
        /// New name of target property
        /// </summary>
        public string Name { get; set; }


        public SerializeAttribute()
        {
        }

        public SerializeAttribute(string name)
        {
            Name = name;
        }            
    }
}
