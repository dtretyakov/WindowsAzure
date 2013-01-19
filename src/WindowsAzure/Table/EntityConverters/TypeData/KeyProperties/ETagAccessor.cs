using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.KeyProperties
{
    /// <summary>
    ///     Handles access to ETag value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class ETagAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _eTagAttributeType;
        private readonly Type _stringType;
        private IValueAccessor<T> _accessor;

        public ETagAccessor()
        {
            _eTagAttributeType = typeof (ETagAttribute);
            _stringType = typeof (String);

            NameChanges = new Dictionary<String, String>();
        }

        public bool HasAccessor
        {
            get { return _accessor != null; }
        }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            if (!Attribute.IsDefined(memberInfo, _eTagAttributeType, false))
            {
                return false;
            }

            if (_accessor != null)
            {
                throw new ArgumentException("ETag attribute duplication.");
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException("ETag must have a String type.");
            }

            _accessor = accessor;

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            if (_accessor != null)
            {
                _accessor.SetValue(entity, new EntityProperty(tableEntity.ETag));
            }
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.ETag = _accessor != null ? _accessor.GetValue(entity).StringValue : "*";
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}