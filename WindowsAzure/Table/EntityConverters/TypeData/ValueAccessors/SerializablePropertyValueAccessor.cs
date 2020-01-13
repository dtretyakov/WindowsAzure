using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    internal class SerializablePropertyValueAccessor<T> : ValueAccessorBase<T>
    {
        /// <summary>
        /// Creates a serializable property value accessor.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        public SerializablePropertyValueAccessor(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            Name = propertyInfo.Name;
            Type = propertyInfo.PropertyType;

            ParameterExpression instanceExpression = Expression.Parameter(typeof(T), "instance");
            MemberExpression memberExpression = Expression.Property(instanceExpression, propertyInfo);

            CreateGetter(instanceExpression, memberExpression);
            CreateSetter(instanceExpression, memberExpression);
        }

        private void CreateSetter(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            ParameterExpression entityPropertyParameter = Expression.Parameter(_entityPropertyType);
            Expression entityPropertyStringValue = Expression.Property(entityPropertyParameter, nameof(EntityProperty.StringValue));

            var methodInfo = typeof(JsonConvert).GetMethods()
               .Single(x =>
                   x.IsStatic
                   && x.IsPublic
                   && x.Name == nameof(JsonConvert.DeserializeObject)    
                   && x.IsGenericMethodDefinition
                   && x.GetParameters().Length == 1)
                .MakeGenericMethod(memberExpression.Type);

            var deserialzieCall = Expression.Call(methodInfo, entityPropertyStringValue);

            SetValue = Expression.Lambda<Action<T, EntityProperty>>(
               Expression.Assign(memberExpression, deserialzieCall),
               instanceExpression, entityPropertyParameter).Compile();
        }

        protected void CreateGetter(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            Expression argumentExpression = memberExpression;
            Type = typeof(string);

            argumentExpression = Serialize(argumentExpression);
            
            ConstructorInfo constructorInfo = _entityPropertyType.GetConstructor(new[] { Type });
            NewExpression newEntityProperty = Expression.New(constructorInfo, argumentExpression);

            GetValue = (Func<T, EntityProperty>)Expression.Lambda(
                newEntityProperty, instanceExpression).Compile();
        }

        protected virtual Expression Serialize(Expression argumentExpression)
        {
            var methodInfo = typeof(JsonConvert).GetMethods()
               .Single(x => 
                   x.IsStatic 
                   && x.IsPublic 
                   && x.Name == nameof(JsonConvert.SerializeObject) 
                   && x.GetParameters().Length == 1);
            var convertedExpression = Expression.Convert(argumentExpression, typeof(object));

            return Expression.Call(methodInfo, convertedExpression);
        }
    }
}
