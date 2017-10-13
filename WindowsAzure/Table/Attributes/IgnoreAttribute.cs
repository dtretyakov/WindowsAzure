using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Defines whether property should not be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreAttribute : Attribute
    {
    }
}