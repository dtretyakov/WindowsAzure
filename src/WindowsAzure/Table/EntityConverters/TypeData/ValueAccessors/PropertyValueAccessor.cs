using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles property value manipulations.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class PropertyValueAccessor<T> : ValueAccessorBase<T>
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

            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;

            ParameterExpression instanceExpression = Expression.Parameter(typeof (T), "instance");
            MemberExpression memberExpression = Expression.Property(instanceExpression, propertyInfo);

            CreateValueAccessors(instanceExpression, memberExpression);
        }
    }
}