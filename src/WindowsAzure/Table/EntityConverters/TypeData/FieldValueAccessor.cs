using System;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    public sealed class FieldValueAccessor<T> : IValueAccessor<T>
    {
        private readonly FieldInfo _fieldInfo;

        public FieldValueAccessor(FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
            {
                throw new ArgumentNullException("fieldInfo");
            }

            _fieldInfo = fieldInfo;

            ParameterExpression instance = Expression.Parameter(typeof (T), "instance");
            ParameterExpression argument = Expression.Parameter(typeof (object), "argument");
            MemberExpression field = Expression.Field(instance, fieldInfo);

            GetValue = (Func<T, object>) Expression.Lambda(
                Expression.Convert(field, typeof (object)),
                instance).Compile();

            SetValue = Expression.Lambda<Action<T, object>>(
                Expression.Assign(field, Expression.Convert(argument, fieldInfo.FieldType)),
                instance, argument).Compile();
        }

        public Func<T, object> GetValue { get; private set; }

        public Action<T, object> SetValue { get; private set; }

        public Type Type
        {
            get { return _fieldInfo.FieldType; }
        }

        public string Name
        {
            get { return _fieldInfo.Name; }
        }
    }
}