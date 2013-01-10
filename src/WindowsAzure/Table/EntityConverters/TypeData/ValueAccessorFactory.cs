using System;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    public static class ValueAccessorFactory
    {
        public static IValueAccessor<T> Create<T>(MemberInfo memberInfo)
        {
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                return new FieldValueAccessor<T>((FieldInfo) memberInfo);
            }

            if (memberInfo.MemberType == MemberTypes.Property)
            {
                return new PropertyValueAccessor<T>((PropertyInfo) memberInfo);
            }

            throw new NotSupportedException(string.Format("{0} member type does not supported.", memberInfo.MemberType));
        }
    }
}