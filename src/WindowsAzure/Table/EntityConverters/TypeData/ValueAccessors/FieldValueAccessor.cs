using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles field value manipulations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class FieldValueAccessor<T> : ValueAccessorBase<T>
    {
        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="fieldInfo">Field info.</param>
        internal FieldValueAccessor(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            Name = fieldInfo.Name;
            Type = fieldInfo.FieldType;

            ParameterExpression instanceExpression = Expression.Parameter(typeof (T), "instance");
            MemberExpression memberExpression = Expression.Field(instanceExpression, fieldInfo);

            CreateValueAccessors(instanceExpression, memberExpression);
        }
    }
}