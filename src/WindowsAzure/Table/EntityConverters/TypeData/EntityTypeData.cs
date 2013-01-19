using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.KeyProperties;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Keeps an entity type data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class EntityTypeData<T> : IEntityTypeData<T> where T : new()
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        private readonly List<IKeyProperty<T>> _keyProperties;
        private readonly List<IValueAccessor<T>> _memberAccessors;
        private readonly IDictionary<string, string> _nameChanges;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public EntityTypeData()
        {
            _memberAccessors = new List<IValueAccessor<T>>();
            _nameChanges = new Dictionary<string, string>();
            _keyProperties = new List<IKeyProperty<T>>
                                 {
                                     new PartitionKeyAccessor<T>(),
                                     new RowKeyAccessor<T>(),
                                     new TimestampAccessor<T>(),
                                     new ETagAccessor<T>()
                                 };

            Type entityType = typeof(T);
            var typeMembers = new List<MemberInfo>(entityType.GetProperties(Flags));
            typeMembers.AddRange(entityType.GetFields(Flags));

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

            foreach (var keyProperty in _keyProperties)
            {
                keyProperty.FillEntity(tableEntity, result);
            }

            foreach (var member in _memberAccessors)
            {
                EntityProperty entityProperty;

                if (tableEntity.Properties.TryGetValue(member.Name, out entityProperty))
                {
                    member.SetValue(result, entityProperty);
                }
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
            var result = new DynamicTableEntity();

            foreach (var keyProperty in _keyProperties)
            {
                keyProperty.FillTableEntity(entity, result);
            }

            foreach (var property in _memberAccessors)
            {
                result.Properties.Add(property.Name, property.GetValue(entity));
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
            foreach (MemberInfo memberInfo in memberInfos)
            {
                IValueAccessor<T> valueAccessor = ValueAccessorFactory.Create<T>(memberInfo);

                if (_keyProperties.Any(p => p.Validate(memberInfo, valueAccessor)))
                {
                    continue;
                }

                // Keep as a regular property
                _memberAccessors.Add(valueAccessor);
            }

            // Get name changes
            foreach (var pair in _keyProperties.SelectMany(keyProperty => keyProperty.NameChanges))
            {
                _nameChanges.Add(pair.Key, pair.Value);
            }

            // Checks for a key attributes
            if (_keyProperties.Count(p => (p is PartitionKeyAccessor<T> || p is RowKeyAccessor<T>) && p.HasAccessor) == 0)
            {
                throw new ArgumentException(
                    string.Format("PartitionKey or RowKey attribute should be defined for type '{0}'.", typeof(T)));
            }
        }
    }
}