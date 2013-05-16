using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to the row key value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class RowKeyProperty<T> : IProperty<T>
    {
        private readonly Func<T, EntityProperty> _getValue;
        private readonly Action<T, EntityProperty> _setValue;
        private readonly Type _stringType = typeof (String);

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="member">Entity member.</param>
        internal RowKeyProperty(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            IValueAccessor<T> accessor = ValueAccessorFactory.Create<T>(member);

            if (accessor.Type != _stringType)
            {
                throw new ArgumentOutOfRangeException(Resources.PropertyRowKeyInvalidType);
            }

            _getValue = accessor.GetValue;
            _setValue = accessor.SetValue;
        }

        /// <summary>
        ///     Sets a POCO member value from table entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <param name="entity">POCO entity.</param>
        public void SetMemberValue(DynamicTableEntity tableEntity, T entity)
        {
            _setValue(entity, new EntityProperty(tableEntity.RowKey));
        }

        /// <summary>
        ///     Gets a POCO member value for table entity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <param name="tableEntity">Table entity.</param>
        public void GetMemberValue(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.RowKey = _getValue(entity).StringValue;
        }
    }
}