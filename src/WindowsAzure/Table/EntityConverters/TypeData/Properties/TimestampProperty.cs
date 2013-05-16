using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to the timestamp value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class TimestampProperty<T> : IProperty<T>
    {
        private readonly Action<T, EntityProperty> _setValue;

        private readonly List<Type> _timestampTypes = new List<Type>
            {
                typeof (DateTime),
                typeof (DateTime?),
                typeof (DateTimeOffset),
                typeof (DateTimeOffset?)
            };

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="member">Entity member.</param>
        internal TimestampProperty(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            IValueAccessor<T> accessor = ValueAccessorFactory.Create<T>(member);

            if (!_timestampTypes.Contains(accessor.Type))
            {
                throw new ArgumentOutOfRangeException(Resources.PropertyTimestampInvalidType);
            }

            _setValue = accessor.SetValue;
        }

        /// <summary>
        ///     Sets a POCO member value from table entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <param name="entity">POCO entity.</param>
        public void SetMemberValue(DynamicTableEntity tableEntity, T entity)
        {
            _setValue(entity, new EntityProperty(tableEntity.Timestamp));
        }

        /// <summary>
        ///     Gets a POCO member value for table entity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <param name="tableEntity">Table entity.</param>
        public void GetMemberValue(T entity, DynamicTableEntity tableEntity)
        {
        }
    }
}