using System;
using System.Reflection;
using WindowsAzure.Common;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Manages creation of the property value accessors.
    /// </summary>
    internal static class ValueAccessorFactory
    {
        /// <summary>
        ///     Creates a value accessor.
        /// </summary>
        /// <typeparam name="T">Type.</typeparam>
        /// <param name="memberInfo">Member info.</param>
        /// <returns>Value accessor.</returns>
        public static IValueAccessor<T> Create<T>(MemberInfo memberInfo)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            var memberType = memberInfo.MemberType();
            if (memberType == MemberTypes.Field)
            {
                return new FieldValueAccessor<T>((FieldInfo) memberInfo);
            }

            if (memberType == MemberTypes.Property)
            {
                return new PropertyValueAccessor<T>((PropertyInfo) memberInfo);
            }

            var message = string.Format(Resources.ValueAccessorFactoryNotSupportedType, memberType);
            throw new NotSupportedException(message);
        }
    }
}