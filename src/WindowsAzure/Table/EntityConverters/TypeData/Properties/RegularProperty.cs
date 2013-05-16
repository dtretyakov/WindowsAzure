using System;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to the property value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class RegularProperty<T> : IProperty<T>
    {
        private readonly Func<T, EntityProperty> _getValue;
        private readonly string _memberName;
        private readonly Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="member">Entity member.</param>
        public RegularProperty(MemberInfo member)
            : this(member, member.Name)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="member">Entity member.</param>
        /// <param name="name">Member name.</param>
        internal RegularProperty(MemberInfo member, string name)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            IValueAccessor<T> accessor = ValueAccessorFactory.Create<T>(member);

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