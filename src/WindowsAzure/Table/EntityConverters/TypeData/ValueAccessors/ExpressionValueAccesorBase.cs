using System;
using System.Linq.Expressions;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Handles access operations to the entity type member via Expressions.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public abstract class ExpressionValueAccesorBase<T> : IValueAccessor<T>
    {
        /// <summary>
        ///     Entity instance expression.
        /// </summary>
        protected ParameterExpression InstanceExpression;

        /// <summary>
        ///     Entity member expression.
        /// </summary>
        protected MemberExpression MemberExpression;

        /// <summary>
        ///     Gets an entity property value.
        /// </summary>
        public Func<T, EntityProperty> GetValue { get; private set; }

        /// <summary>
        ///     Sets an entity property value.
        /// </summary>
        public Action<T, EntityProperty> SetValue { get; private set; }

        /// <summary>
        /// Member type.
        /// </summary>
        public abstract Type Type { get; protected set; }

        /// <summary>
        /// Member name.
        /// </summary>
        public abstract string Name { get; protected set; }

        /// <summary>
        ///     Initializes a get and set value methods.
        /// </summary>
        protected void Initialize()
        {
            ParameterExpression entityPropertyParameter = Expression.Parameter(typeof (EntityProperty));

            // Gets a member base type name
            string typeName = Type.Name;
            if (Type.IsValueType)
            {
                Type baseType = Nullable.GetUnderlyingType(Type);
                if (baseType != null)
                {
                    typeName = baseType.Name;
                }
            }

            MethodCallExpression createEntityProperty;

            try
            {
                createEntityProperty = Expression.Call(
                typeof(EntityPropertyFactory).GetMethod("Create", new[] { Type }),
                MemberExpression);
            }
            catch (Exception e)
            {
                var message = String.Format(Resources.ExpressionValueAccessorInvalidMemberType, Type);
                throw new ArgumentException(message, e);
            }

            Expression entityPropertyValue;

            if (typeName == "DateTime")
            {
                var dateTimeOffsetProperty = Expression.Property(entityPropertyParameter, "DateTimeOffsetValue");
                var hasValueProperty = Expression.Property(dateTimeOffsetProperty, "HasValue");
                var valueProperty = Expression.Property(dateTimeOffsetProperty, "Value");
                var dateTimeProperty = Expression.Property(valueProperty, "DateTime");

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
                createEntityProperty,
                InstanceExpression).Compile();

            SetValue = Expression.Lambda<Action<T, EntityProperty>>(
                Expression.Assign(MemberExpression, convertType),
                InstanceExpression, entityPropertyParameter).Compile();
        }
    }
}