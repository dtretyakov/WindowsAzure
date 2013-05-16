using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Defines a custom property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PropertyAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="name">Custom property name.</param>
        public PropertyAttribute(string name)
        {
            _name = name;
        }

        /// <summary>
        ///     Gets a custom property name.
        /// </summary>
        public string Name
        {
            get { return _name; }
        }
    }
}