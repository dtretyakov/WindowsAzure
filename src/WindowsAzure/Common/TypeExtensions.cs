using System;
using System.Reflection;

namespace WindowsAzure.Common
{
#if NETCORE
    internal enum MemberTypes
    {
        Property = 0,
        Field = 1,
        Event = 2,
        Method = 3,
        Other = 4
    }
#endif

    /// <summary>
    /// Reflection utilities.
    /// </summary>
    internal static class TypeExtensions
    {
#if NET40
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }
#endif

        public static bool IsAbstractType(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            return !typeInfo.IsClass || typeInfo.IsAbstract;
        }

        public static MemberTypes MemberType(this MemberInfo memberInfo)
        {
#if NET40
            return memberInfo.MemberType;
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
#endif
        }

#if NETCORE
        public static InterfaceMapping GetInterfaceMap(this Type type, Type interfaceType)
        {
            return type.GetTypeInfo().GetRuntimeInterfaceMap(interfaceType);
        }
#endif
    }
}
