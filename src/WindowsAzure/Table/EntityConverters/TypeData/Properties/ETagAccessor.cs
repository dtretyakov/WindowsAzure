using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to ETag value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class ETagAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _eTagAttributeType;
        private readonly Type _stringType;
        private Func<T, EntityProperty> _getValue;
        private Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal ETagAccessor()
        {
            _eTagAttributeType = typeof (ETagAttribute);
            _stringType = typeof (String);

            NameChanges = new Dictionary<String, String>();
        }

        public bool HasAccessor { get; private set; }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            if (!Attribute.IsDefined(memberInfo, _eTagAttributeType, false))
            {
                return false;
            }

            if (HasAccessor)
            {
                throw new ArgumentException(Resources.ETagAttributeDuplicate);
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException(Resources.ETagInvalidType);
            }

            HasAccessor = true;

            _getValue = accessor.GetValue;
            _setValue = accessor.SetValue;

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            _setValue(entity, new EntityProperty(tableEntity.ETag));
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.ETag = _getValue(entity).StringValue;
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}