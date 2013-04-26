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
    ///     Handles access to Timestamp value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class TimestampAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _timestampAttributeType = typeof (TimestampAttribute);

        private readonly List<Type> _timestampTypes = new List<Type>
            {
                typeof (DateTime),
                typeof (DateTime?),
                typeof (DateTimeOffset),
                typeof (DateTimeOffset?)
            };

        private Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal TimestampAccessor()
        {
            NameChanges = new Dictionary<string, string>();
        }

        public bool HasAccessor { get; private set; }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            if (!Attribute.IsDefined(memberInfo, _timestampAttributeType, false))
            {
                return false;
            }

            if (HasAccessor)
            {
                throw new ArgumentException(Resources.TimestampAttributeDuplicate);
            }

            if (!_timestampTypes.Contains(accessor.Type))
            {
                throw new ArgumentException(Resources.TimestampInvalidType);
            }

            HasAccessor = true;

            _setValue = accessor.SetValue;

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            _setValue(entity, new EntityProperty(tableEntity.Timestamp));
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}