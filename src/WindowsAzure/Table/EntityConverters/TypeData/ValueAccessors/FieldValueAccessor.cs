using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles field value manipulations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class FieldValueAccessor<T> : ExpressionValueAccesorBase<T>
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

            InstanceExpression = Expression.Parameter(typeof (T), "instance");
            MemberExpression = Expression.Field(InstanceExpression, fieldInfo);

            Name = fieldInfo.Name;
            Type = fieldInfo.FieldType;

            Initialize();
        }

        /// <summary>
        ///     Field type.
        /// </summary>
        public override Type Type { get; protected set; }

        /// <summary>
        ///     Field name.
        /// </summary>
        public override string Name { get; protected set; }
    }
}