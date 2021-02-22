using System;
using System.Linq.Expressions;
using System.Reflection;
#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#else
using Microsoft.Azure.Cosmos.Table;
#endif
using WindowsAzure.Common;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles access operations to the entity type member via Expressions.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal abstract class ValueAccessorBase<T> : IValueAccessor<T>
    {
        protected readonly Type _entityPropertyType = typeof (EntityProperty);

        /// <summary>
        ///     Gets an entity member accessor.
        /// </summary>
        public Func<T, EntityProperty> GetValue { get; protected set; }

        /// <summary>
        ///     Sets an entity member mutator.
        /// </summary>
        public Action<T, EntityProperty> SetValue { get; protected set; }

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
        protected virtual void CreateValueAccessors(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            Expression argumentExpression = memberExpression;

            // override type if is an enum
            Type baseType = Type;
            var typeInfo = Type.GetTypeInfo();
            bool isEnum = typeInfo.IsEnum;

            // Get base type name
            string typeName = Type.Name;
            if (typeInfo.IsValueType)
            {
                Type nullableType = Nullable.GetUnderlyingType(Type);
                if (nullableType != null)
                {
                    typeName = nullableType.Name;
                    var nullableTypeInfo = nullableType.GetTypeInfo();
                    isEnum = nullableTypeInfo.IsEnum; // We set this variable to use bellow
                    if (isEnum)
                    {
                        Type = nullableType;
                    }
                }

                // Override the member type to use the Enum base type
                if (isEnum)
                {
                    Type = Enum.GetUnderlyingType(Type);
                    typeName = Type.Name;
                }

                // As EntityProperty contains only nullable ctors for value types
                // we must convert the type to a nullable base type
                if (isEnum || nullableType == null)
                {                    
                    nullableType = typeof(Nullable<>).MakeGenericType(Type);
                    argumentExpression = Expression.Convert(memberExpression, nullableType);
                }
            }

            // Execute EntityProperty constructor
            ConstructorInfo constructorInfo = _entityPropertyType.GetConstructor(new[] {Type});
            if (constructorInfo == null)
            {
                string message = string.Format(Resources.ExpressionValueAccessorInvalidMemberType, Type);
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
                entityPropertyValue = Expression.Property(entityPropertyParameter, $"{typeName}Value");
            }

            UnaryExpression convertType = Expression.Convert(entityPropertyValue, Type);

            // We need an additional conversion if the property is an enum
            if (isEnum)
            {
                convertType = Expression.Convert(convertType, baseType);
            }

            GetValue = (Func<T, EntityProperty>) Expression.Lambda(
                newEntityProperty, instanceExpression).Compile();

            SetValue = Expression.Lambda<Action<T, EntityProperty>>(
                Expression.Assign(memberExpression, convertType),
                instanceExpression, entityPropertyParameter).Compile();
        }
    }
}