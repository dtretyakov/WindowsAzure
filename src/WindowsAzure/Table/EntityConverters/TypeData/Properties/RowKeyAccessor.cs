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
    ///     Handles access to RowKey value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class RowKeyAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _rowKeyAttributeType = typeof (RowKeyAttribute);
        private readonly Type _stringType = typeof (String);
        private Func<T, EntityProperty> _getValue;
        private Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal RowKeyAccessor()
        {
            NameChanges = new Dictionary<string, string>();
        }

        public bool HasAccessor { get; private set; }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            // Checks member for a row key attribute
            if (!Attribute.IsDefined(memberInfo, _rowKeyAttributeType, false))
            {
                return false;
            }

            if (HasAccessor)
            {
                throw new ArgumentException(Resources.RowKeyAttributeDuplicate);
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException(Resources.RowKeyInvalidType);
            }

            HasAccessor = true;

            _getValue = accessor.GetValue;
            _setValue = accessor.SetValue;

            NameChanges.Add(accessor.Name, "RowKey");

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            _setValue(entity, new EntityProperty(tableEntity.RowKey));
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.RowKey = _getValue(entity).StringValue;
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}