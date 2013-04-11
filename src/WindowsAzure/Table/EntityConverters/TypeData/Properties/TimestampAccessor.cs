using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to Timestamp value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class TimestampAccessor<T> : IKeyProperty<T>
    {
        private readonly Type _timestampAttributeType = typeof (TimestampAttribute);
        private readonly List<Type> _timestampTypes;
        private IValueAccessor<T> _accessor;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public TimestampAccessor()
        {
            _timestampAttributeType = typeof (TimestampAttribute);

            _timestampTypes = new List<Type>
                {
                    typeof (DateTime),
                    typeof (DateTime?),
                    typeof (DateTimeOffset),
                    typeof (DateTimeOffset?)
                };

            NameChanges = new Dictionary<string, string>();
        }

        public bool HasAccessor
        {
            get { return _accessor != null; }
        }

        public bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            if (!Attribute.IsDefined(memberInfo, _timestampAttributeType, false))
            {
                return false;
            }

            if (_accessor != null)
            {
                throw new ArgumentException("Timestamp attribute duplication.");
            }

            if (!_timestampTypes.Contains(accessor.Type))
            {
                throw new ArgumentException("Timestamp should be a DateTimeOffset type.");
            }

            _accessor = accessor;

            return true;
        }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            if (_accessor != null)
            {
                _accessor.SetValue(entity, new EntityProperty(tableEntity.Timestamp));
            }
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
        }

        public IDictionary<string, string> NameChanges { get; private set; }
    }
}