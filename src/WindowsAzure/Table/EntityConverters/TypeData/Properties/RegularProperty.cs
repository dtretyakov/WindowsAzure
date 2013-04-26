using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to regular property.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class RegularProperty<T> : IProperty<T>
    {
        private readonly Func<T, EntityProperty> _getValue;
        private readonly string _memberName;
        private readonly Dictionary<string, string> _nameChanges;
        private readonly Action<T, EntityProperty> _setValue;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="memberInfo">Member info.</param>
        /// <param name="accessor">Value accessor.</param>
        internal RegularProperty(MemberInfo memberInfo, IValueAccessor<T> accessor)
        {
            _nameChanges = new Dictionary<string, string>();

            // Skip entity members with a IgnoreAttribute
            if (Attribute.GetCustomAttribute(memberInfo, typeof (IgnoreAttribute)) != null)
            {
                return;
            }

            HasAccessor = true;

            _getValue = accessor.GetValue;
            _setValue = accessor.SetValue;
            _memberName = memberInfo.Name;

            // Try to get PropertyAttribute
            var propertyAttribute = Attribute.GetCustomAttribute(memberInfo, typeof (PropertyAttribute)) as PropertyAttribute;
            if (propertyAttribute == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(propertyAttribute.Name))
            {
                return;
            }

            _nameChanges.Add(_memberName, propertyAttribute.Name);
            _memberName = propertyAttribute.Name;
        }

        public IDictionary<string, string> NameChanges
        {
            get { return _nameChanges; }
        }

        public bool HasAccessor { get; private set; }

        public void FillEntity(DynamicTableEntity tableEntity, T entity)
        {
            EntityProperty entityProperty;

            if (tableEntity.Properties.TryGetValue(_memberName, out entityProperty))
            {
                _setValue(entity, entityProperty);
            }
        }

        public void FillTableEntity(T entity, DynamicTableEntity tableEntity)
        {
            tableEntity.Properties.Add(_memberName, _getValue(entity));
        }
    }
}