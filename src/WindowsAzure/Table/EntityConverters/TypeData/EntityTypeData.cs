using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Keeps an entity type data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal sealed class EntityTypeData<T> : IEntityTypeData<T> where T : class, new()
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        private readonly IDictionary<string, string> _nameChanges;
        private readonly List<IProperty<T>> _properties;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal EntityTypeData()
        {
            _nameChanges = new Dictionary<string, string>();
            _properties = new List<IProperty<T>>();

            Type entityType = typeof (T);

            var typeMembers = new List<MemberInfo>(entityType.GetFields(Flags));
            typeMembers.AddRange(entityType.GetProperties(Flags).Where(p => p.CanRead && p.CanWrite));

            ProcessMembers(typeMembers);
        }

        /// <summary>
        ///     Converts DynamicTableEntity into POCO entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>POCO entity.</returns>
        public T GetEntity(DynamicTableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException("tableEntity");
            }

            var result = new T();

            foreach (var property in _properties)
            {
                property.FillEntity(tableEntity, result);
            }

            return result;
        }

        /// <summary>
        ///     Converts POCO entity into ITableEntity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <returns>Table entity.</returns>
        public ITableEntity GetEntity(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var result = new DynamicTableEntity();

            foreach (var property in _properties)
            {
                property.FillTableEntity(entity, result);
            }

            return result;
        }

        /// <summary>
        ///     Gets an entity members name changes.
        /// </summary>
        public IDictionary<string, string> NameChanges
        {
            get { return _nameChanges; }
        }

        /// <summary>
        ///     Processes type members.
        /// </summary>
        /// <param name="memberInfos">Type memebers.</param>
        private void ProcessMembers(IEnumerable<MemberInfo> memberInfos)
        {
            // List of available key properties
            var keyProperties = new List<IKeyProperty<T>>
                {
                    new PartitionKeyAccessor<T>(),
                    new RowKeyAccessor<T>(),
                    new TimestampAccessor<T>(),
                    new ETagAccessor<T>()
                };

            // Create accessors for entity members
            foreach (MemberInfo memberInfo in memberInfos)
            {
                IValueAccessor<T> valueAccessor = ValueAccessorFactory.Create<T>(memberInfo);

                if (keyProperties.Any(p => p.Validate(memberInfo, valueAccessor)))
                {
                    continue;
                }

                // Keep as a regular property
                var property = new RegularProperty<T>(memberInfo, valueAccessor);
                if (property.HasAccessor)
                {
                    _properties.Add(property);
                }
            }

            // At least one key property should be defined
            if (keyProperties.Count(p => p.HasAccessor && (p is PartitionKeyAccessor<T> || p is RowKeyAccessor<T>)) == 0)
            {
                string message = string.Format(Resources.EntityTypeDataMissingKey, typeof (T));
                throw new ArgumentException(message);
            }

            // Merge properties
            _properties.AddRange(keyProperties);

            // Get name changes
            foreach (var pair in _properties.SelectMany(keyProperty => keyProperty.NameChanges))
            {
                _nameChanges.Add(pair.Key, pair.Value);
            }
        }
    }
}