using System;

namespace WindowsAzure.Table.Attributes
{
    /// <summary>
    ///     Defines whether property keep partition key value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public sealed class PartitionKeyAttribute : Attribute
    {
    }
}