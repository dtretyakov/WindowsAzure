using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Defines whether property should keep an etag value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class ETagAttribute : Attribute
    {
    }
}