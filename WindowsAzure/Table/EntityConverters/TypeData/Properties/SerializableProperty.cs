using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.Serializers;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    internal sealed class SerializableProperty<T> : IProperty<T>
    {
        private readonly Func<T, EntityProperty> _getValue;
        private readonly string _memberName;
        private readonly Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="member">Entity member.</param>
        public SerializableProperty(MemberInfo member)
            : this(member, member.Name)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="name">Member name.</param>
        internal SerializableProperty(MemberInfo member, string name)
        {
            if (member == null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            IValueAccessor<T> accessor = new SerializablePropertyValueAccessor<T>((PropertyInfo)member, SerializationSettings.Instance.Default);

            _getValue = accessor.GetValue;
            _setValue = accessor.SetValue;
            _memberName = name;
        }

        /// <summary>
        ///     Sets a POCO member value from table entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <param name="entity">POCO entity.</param>
        public void SetMemberValue(DynamicTableEntity tableEntity, T entity)
        {
            EntityProperty entityProperty;

            if (tableEntity.Properties.TryGetValue(_memberName, out entityProperty))
            {
                _setValue(entity, entityProperty);
            }
        }

        /// <summary>
        ///     Gets a POCO member value for table entity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <param name="tableEntity">Table entity.</param>
        public void GetMemberValue(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.Properties.Add(_memberName, _getValue(entity));
        }
    }
}