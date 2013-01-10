using System;

namespace WindowsAzure.Table.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RowKeyAttribute : Attribute
    {
    }
}