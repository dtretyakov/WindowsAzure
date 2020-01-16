using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    internal class SerializablePropertyValueAccessor<T> : ValueAccessorBase<T>
    {
        private readonly Type _serializerType;

        /// <summary>
        /// Creates a serializable property value accessor.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        internal SerializablePropertyValueAccessor(PropertyInfo propertyInfo, ISerializer serializer)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            _serializerType = serializer.GetType() ?? throw new ArgumentNullException(nameof(serializer));

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

            var deserializeMethod = CreateDeserializeMethod(memberExpression.Type);

            var serializerInstanceExpression = Expression.New(_serializerType);
            var deserialzieCall = Expression.Call(serializerInstanceExpression, deserializeMethod, entityPropertyStringValue);

            SetValue = Expression.Lambda<Action<T, EntityProperty>>(
               Expression.Assign(memberExpression, deserialzieCall),
               instanceExpression, entityPropertyParameter).Compile();
        }

        private void CreateGetter(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            Expression argumentExpression = memberExpression;

            var serializeMethod = CreateSerializeMethod();
            
            var convertedExpression = Expression.Convert(argumentExpression, typeof(object));

            var serializerInstanceExpression = Expression.New(_serializerType);
            argumentExpression = Expression.Call(serializerInstanceExpression, serializeMethod, convertedExpression);
            
            var constructorInfo = _entityPropertyType.GetConstructor(new[] { typeof(string) });
            NewExpression newEntityProperty = Expression.New(constructorInfo, argumentExpression);

            GetValue = (Func<T, EntityProperty>)Expression.Lambda(
                newEntityProperty, instanceExpression).Compile();
        }

        private MethodInfo CreateDeserializeMethod(Type deserializeType)
        {
            return GetSerializerMethod(x => x.Name == nameof(ISerializer.Deserialize)
                            && x.IsGenericMethodDefinition)
                .MakeGenericMethod(deserializeType);
        }

        private MethodInfo CreateSerializeMethod()
        {
            return GetSerializerMethod(x => x.Name == nameof(ISerializer.Serialize));
        }

        private MethodInfo GetSerializerMethod(Func<MethodInfo, bool> predicate)
        {
            return _serializerType
                .GetMethods()
                .Single(x =>
                   x.IsPublic
                   && x.GetParameters().Length == 1
                   && predicate(x));
        }
    }
}
