using System;

namespace GitHub.WindowsAzure.Table
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class TableKeyAttribute : Attribute
    {
    }
}