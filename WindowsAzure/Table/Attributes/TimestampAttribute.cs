using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Defines whether property should keep a timestamp value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class TimestampAttribute : Attribute
    {
    }
}