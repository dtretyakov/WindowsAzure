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
    ///     Handles access to PartitionKey value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class PartitionKeyAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _partitionKeyAttributeType;
        private readonly Type _stringType;
        private Func<T, EntityProperty> _getValue;
        private Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal PartitionKeyAccessor()
        {
            _partitionKeyAttributeType = typeof (PartitionKeyAttribute);
            _stringType = typeof (String);

            NameChanges = new Dictionary<string, string>();
        }

        public bool HasAccessor { get; private set; }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            // Checks member for a partition key attribute
            if (!Attribute.IsDefined(memberInfo, _partitionKeyAttributeType, false))
            {
                return false;
            }

            if (HasAccessor)
            {
                throw new ArgumentException(Resources.PartitionKeyAttributeDuplicate);
            }

            if (accessor.Type != _stringType)
            {
                throw new ArgumentException(Resources.PartitionKeyInvalidType);
            }

            HasAccessor = true;

            _getValue = accessor.GetValue;
            _setValue = accessor.SetValue;

            NameChanges.Add(accessor.Name, "PartitionKey");

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            _setValue(entity, new EntityProperty(tableEntity.PartitionKey));
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.PartitionKey = _getValue(entity).StringValue;
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}