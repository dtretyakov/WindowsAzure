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
        private IValueAccessor<T> _accessor;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal RowKeyAccessor()
        {
            NameChanges = new Dictionary<string, string>();
        }

        public bool HasAccessor
        {
            get { return _accessor != null; }
        }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            // Checks member for a row key attribute
            if (!Attribute.IsDefined(memberInfo, _rowKeyAttributeType, false))
            {
                return false;
            }

            if (_accessor != null)
            {
                throw new ArgumentException(Resources.RowKeyAttributeDuplicate);
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException(Resources.RowKeyInvalidType);
            }

            _accessor = accessor;

            NameChanges.Add(accessor.Name, "RowKey");

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            if (_accessor != null)
            {
                _accessor.SetValue(entity, new EntityProperty(tableEntity.RowKey));
            }
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            if (_accessor != null)
            {
                tableEntity.RowKey = _accessor.GetValue(entity).StringValue;
            }

            tableEntity.RowKey = tableEntity.RowKey ?? string.Empty;
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}