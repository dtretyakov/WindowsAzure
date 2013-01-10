using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    public sealed class PropertyValueAccessor<T> : IValueAccessor<T>
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyValueAccessor(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;

            var instance = Expression.Parameter(typeof(T), "instance");

            var property = Expression.Property(instance, propertyInfo);
            var convert = Expression.TypeAs(property, typeof(object));

            GetValue = (Func<T, object>)Expression.Lambda(convert, instance).Compile();

            var argument = Expression.Parameter(typeof(object), "argument");

            SetValue = Expression.Lambda<Action<T, object>>(
                Expression.Assign(property, Expression.Convert(argument, propertyInfo.PropertyType)),
                instance, argument).Compile();
        }

        public Func<T, object> GetValue { get; private set; }

        public Action<T, object> SetValue { get; private set; }

        public string Name
        {
            get { return _propertyInfo.Name; }
        }

        public Type Type
        {
            get { return _propertyInfo.PropertyType; }
        }
    }
}