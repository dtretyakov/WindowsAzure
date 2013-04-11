using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles property value manipulations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class PropertyValueAccessor<T> : ExpressionValueAccesorBase<T>
    {
        /// <summary>
        ///     Creates a property value accessor.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        public PropertyValueAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            InstanceExpression = Expression.Parameter(typeof (T), "instance");
            MemberExpression = Expression.Property(InstanceExpression, propertyInfo);

            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;

            Initialize();
        }

        /// <summary>
        ///     Property type.
        /// </summary>
        public override Type Type { get; protected set; }

        /// <summary>
        ///     Property name.
        /// </summary>
        public override string Name { get; protected set; }
    }
}