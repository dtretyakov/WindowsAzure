using System;
using System.Collections.Generic;
using System.Reflection;

namespace WindowsAzure.Table.Extensions
{
    internal static class TypeExtentions
    {
        private static readonly HashSet<Type> _supportedEntityPropertyTypes = new HashSet<Type>
        {
            typeof(long),
            typeof(int),
            typeof(Guid),
            typeof(double),
            typeof(string),
            typeof(bool),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(byte[]),
        };

        public static bool IsSupportedEntityPropertyType(this Type type)
        {
            if (type.GetTypeInfo().IsEnum)
            {
                return _supportedEntityPropertyTypes.Contains(Enum.GetUnderlyingType(type));
            }

            return _supportedEntityPropertyTypes.Contains(type)
                || _supportedEntityPropertyTypes.Contains(Nullable.GetUnderlyingType(type));
        }
    }
}
