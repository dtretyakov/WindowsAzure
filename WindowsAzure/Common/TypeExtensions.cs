using System;
using System.Collections.Generic;
using System.Reflection;

namespace WindowsAzure.Common
{

    /// <summary>
    /// Reflection utilities.
    /// </summary>
    internal static class TypeExtensions
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

        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                return propertyInfo.PropertyType;
            }

            else if (memberInfo is FieldInfo fieldInfo)
            {
                return fieldInfo.FieldType;
            }

            throw new ArgumentException($"Unsupported target member type {memberInfo.MemberType().ToString()}.");
        }

        public static bool IsAbstractType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsClass || typeInfo.IsAbstract;
        }

        public static MemberTypes MemberType(this MemberInfo memberInfo)
        {
#if NET452 || NET46 || NETSTANDARD2_0
            return memberInfo.MemberType;
        }
    }
#else
            if (memberInfo is PropertyInfo)
            {
                return MemberTypes.Property;
            }
            else if (memberInfo is FieldInfo)
            {
                return MemberTypes.Field;
            }
            else if (memberInfo is EventInfo)
            {
                return MemberTypes.Event;
            }
            else if (memberInfo is MethodInfo)
            {
                return MemberTypes.Method;
            }
            else
            {
                return MemberTypes.Other;
            }
        }

        public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
        {
            return type.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
        }
    }

#if !NETSTANDARD2_0
    internal enum MemberTypes
    {
        Property = 0,
        Field = 1,
        Event = 2,
        Method = 3,
        Other = 4
    }
#endif
#endif
}