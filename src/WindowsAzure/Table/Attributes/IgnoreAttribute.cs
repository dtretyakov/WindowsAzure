using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Defines that property should not be serialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class IgnoreAttribute : Attribute
    {
    }
}