using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Allows to change custom serialization property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets a custom property name.
        /// </summary>
        public string Name { get; set; }
    }
}