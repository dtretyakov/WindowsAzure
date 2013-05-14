using System;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles access operations to the entity type member via Expressions.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal abstract class ValueAccessorBase<T> : IValueAccessor<T>
    {
        private readonly Type _entityPropertyType = typeof (EntityProperty);

        /// <summary>
        ///     Gets an entity member accessor.
        /// </summary>
        public Func<T, EntityProperty> GetValue { get; private set; }

        /// <summary>
        ///     Sets an entity member mutator.
        /// </summary>
        public Action<T, EntityProperty> SetValue { get; private set; }

        /// <summary>
        ///     Gets a member type.
        /// </summary>
        public Type Type { get; protected set; }

        /// <summary>
        ///     Gets a member name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        ///     Initializes a value accessor.
        /// </summary>
        protected void CreateValueAccessors(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            Expression argumentExpression = memberExpression;

            // Get base type name
            string typeName = Type.Name;
            if (Type.IsValueType)
            {
                Type baseType = Nullable.GetUnderlyingType(Type);
                if (baseType != null)
                {
                    typeName = baseType.Name;
                }
                else
                {
                    // EntityProperty contains only nullable ctors for value types
                    Type nullableType = typeof (Nullable<>).MakeGenericType(new[] {Type});
                    argumentExpression = Expression.Convert(memberExpression, nullableType);
                }
            }

            // Execute EntityProperty constructor
            ConstructorInfo constructorInfo = _entityPropertyType.GetConstructor(new[] {Type});
            if (constructorInfo == null)
            {
                string message = String.Format(Resources.ExpressionValueAccessorInvalidMemberType, Type);
                throw new ArgumentException(message);
            }

            NewExpression newEntityProperty = Expression.New(constructorInfo, argumentExpression);

            // Access EntityProperty value
            Expression entityPropertyValue;
            ParameterExpression entityPropertyParameter = Expression.Parameter(_entityPropertyType);

            if (typeName == "DateTime")
            {
                MemberExpression dateTimeOffsetProperty = Expression.Property(entityPropertyParameter, "DateTimeOffsetValue");
                MemberExpression hasValueProperty = Expression.Property(dateTimeOffsetProperty, "HasValue");
                MemberExpression valueProperty = Expression.Property(dateTimeOffsetProperty, "Value");
                MemberExpression dateTimeProperty = Expression.Property(valueProperty, "DateTime");

                entityPropertyValue = Expression.Condition(
                    Expression.IsTrue(hasValueProperty),
                    dateTimeProperty,
                    Expression.Constant(DateTime.MinValue));
            }
            else if (typeName == "Byte[]")
            {
                entityPropertyValue = Expression.Property(entityPropertyParameter, "BinaryValue");
            }
            else
            {
                entityPropertyValue = Expression.Property(entityPropertyParameter, string.Format("{0}Value", typeName));
            }

            UnaryExpression convertType = Expression.Convert(entityPropertyValue, Type);

            GetValue = (Func<T, EntityProperty>) Expression.Lambda(
                newEntityProperty, instanceExpression).Compile();

            SetValue = Expression.Lambda<Action<T, EntityProperty>>(
                Expression.Assign(memberExpression, convertType),
                instanceExpression, entityPropertyParameter).Compile();
        }
    }
}