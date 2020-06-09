#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#else
using Microsoft.Azure.Cosmos.Table;
#endif
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    internal sealed class SerializableValueAccessor<T> : ValueAccessorBase<T>
    {
        private readonly ISerializer _serializer;

        /// <summary>
        /// Creates a serializable property value accessor.
        /// </summary>
        /// <param name="propertyInfo">Property info.</param>
        /// <param name="serializer">Serializer.</param>
        internal SerializableValueAccessor(MemberInfo memberInfo, ISerializer serializer)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException(nameof(memberInfo));
            }

            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            ParameterExpression instanceExpression = Expression.Parameter(typeof(T), "instance");

            CreateValueAccessors(instanceExpression, CreateMemberExpression(memberInfo, instanceExpression));
        }        

        protected override void CreateValueAccessors(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            CreateGetter(instanceExpression, memberExpression);
            CreateSetter(instanceExpression, memberExpression);
        }       

        private void CreateSetter(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            ParameterExpression entityPropertyParameter = Expression.Parameter(_entityPropertyType);
            Expression entityPropertyStringValue = Expression.Property(entityPropertyParameter, nameof(EntityProperty.StringValue));

            var deserialzieCall = Expression.Call(Expression.Constant(_serializer), CreateDeserializeMethod(Type), entityPropertyStringValue);

            SetValue = Expression.Lambda<Action<T, EntityProperty>>(
               Expression.Assign(memberExpression, deserialzieCall),
               instanceExpression, entityPropertyParameter).Compile();
        }

        private void CreateGetter(ParameterExpression instanceExpression, MemberExpression memberExpression)
        {
            Expression argumentExpression = memberExpression;
            
            var convertedExpression = Expression.Convert(argumentExpression, typeof(object));

            argumentExpression = Expression.Call(Expression.Constant(_serializer), CreateSerializeMethod(_serializer.Serialize), convertedExpression);
            
            var constructorInfo = _entityPropertyType.GetConstructor(new[] { typeof(string) });
            NewExpression newEntityProperty = Expression.New(constructorInfo, argumentExpression);

            GetValue = (Func<T, EntityProperty>)Expression.Lambda(
                newEntityProperty, instanceExpression).Compile();
        }        

        private MethodInfo CreateDeserializeMethod(Type deserializeType)
        {
           return _serializer
                .GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Single(x =>
                {
                    var parameters = x.GetParameters();

                    return x.Name == nameof(ISerializer.Deserialize)
                                   && parameters.Length == 1
                                   && (parameters[0]).ParameterType == typeof(string)
                                   && x.IsGenericMethodDefinition;
                })                   
                .MakeGenericMethod(deserializeType);               
        }

        private MethodInfo CreateSerializeMethod(Func<object, string> serializeMethod) => serializeMethod.GetMethodInfo();

        private MemberExpression CreateMemberExpression(MemberInfo memberInfo, ParameterExpression instanceExpression)
        {
            if (memberInfo is PropertyInfo propertyInfo)
            {
                Type = propertyInfo.PropertyType;
                return Expression.Property(instanceExpression, propertyInfo);
            }

            else if (memberInfo is FieldInfo fieldInfo)
            {
                Type = fieldInfo.FieldType;
                return Expression.Field(instanceExpression, fieldInfo);
            }

            throw new ArgumentException(nameof(memberInfo));
        }
    }
}
