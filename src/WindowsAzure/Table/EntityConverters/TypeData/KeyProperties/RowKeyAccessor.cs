using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.KeyProperties
{
    /// <summary>
    ///     Handles access to RowKey value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class RowKeyAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _rowKeyAttributeType;
        private readonly Type _stringType;
        private IValueAccessor<T> _accessor;

        public RowKeyAccessor()
        {
            _rowKeyAttributeType = typeof (RowKeyAttribute);
            _stringType = typeof (String);

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
                throw new ArgumentException("RowKey attribute duplication.");
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException("RowKey must have a string type.");
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
            tableEntity.RowKey = _accessor != null ? _accessor.GetValue(entity).StringValue : string.Empty;
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}