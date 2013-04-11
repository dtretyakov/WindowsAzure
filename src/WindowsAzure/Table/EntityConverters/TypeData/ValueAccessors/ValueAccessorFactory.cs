using System;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Manages creation of the property value accessors.
    /// </summary>
    public static class ValueAccessorFactory
    {
        /// <summary>
        ///     Creates a value accessor.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="memberInfo">Member info.</param>
        /// <returns>Value accessor.</returns>
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

            throw new NotSupportedException(string.Format("Member type '{0}' does not supported.", memberInfo.MemberType));
        }
    }
}