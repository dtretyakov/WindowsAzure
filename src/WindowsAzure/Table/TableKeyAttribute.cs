using System;

namespace WindowsAzure.Table
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TableKeyAttribute : Attribute
    {
    }
}