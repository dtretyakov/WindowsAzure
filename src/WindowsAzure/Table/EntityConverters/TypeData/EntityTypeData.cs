using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Attributes;
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

        private readonly Type _datetimeType = typeof (DateTime);
        private readonly Type _etagAttributeType = typeof (ETagAttribute);
        private readonly List<IValueAccessor<T>> _memberAccessors;
        private readonly Dictionary<String, String> _nameChanges;
        private readonly Type _partitionKeyAttributeType = typeof (PartitionKeyAttribute);
        private readonly Type _rowKeyAttributeType = typeof (RowKeyAttribute);
        private readonly Type _stringType = typeof (String);
        private readonly Type _timestampAttributeType = typeof (TimestampAttribute);
        private IValueAccessor<T> _etag;
        private IValueAccessor<T> _partitionKey;
        private IValueAccessor<T> _rowKey;
        private IValueAccessor<T> _timestamp;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public EntityTypeData()
        {
            Type entityType = typeof (T);

            _nameChanges = new Dictionary<string, string>();
            _memberAccessors = new List<IValueAccessor<T>>();

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

            if (_partitionKey != null)
            {
                _partitionKey.SetValue(result, new EntityProperty(tableEntity.PartitionKey));
            }

            if (_rowKey != null)
            {
                _rowKey.SetValue(result, new EntityProperty(tableEntity.RowKey));
            }

            if (_etag != null)
            {
                _etag.SetValue(result, new EntityProperty(tableEntity.ETag));
            }

            if (_timestamp != null)
            {
                _timestamp.SetValue(result, new EntityProperty(tableEntity.Timestamp));
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
            var result = new DynamicTableEntity
                             {
                                 PartitionKey =
                                     _partitionKey != null ? _partitionKey.GetValue(entity).StringValue : string.Empty,
                                 RowKey = _rowKey != null ? _rowKey.GetValue(entity).StringValue : string.Empty,
                                 ETag = _etag != null ? _etag.GetValue(entity).StringValue : "*"
                             };

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
        ///     Processes partition key accessor.
        /// </summary>
        /// <param name="valueAccessor">Partition key accessor.</param>
        private void ProcessPartitionKey(IValueAccessor<T> valueAccessor)
        {
            if (_partitionKey != null)
            {
                throw new ArgumentException("PartitionKey attribute duplication.");
            }

            if (valueAccessor.Type != _stringType)
            {
                throw new ArgumentException("PartitionKey must have a string type.");
            }

            _partitionKey = valueAccessor;

            _nameChanges.Add(valueAccessor.Name, "PartitionKey");
        }

        /// <summary>
        ///     Processes row ket accessor.
        /// </summary>
        /// <param name="valueAccessor">Row ket accessor.</param>
        private void ProcessRowKey(IValueAccessor<T> valueAccessor)
        {
            if (_rowKey != null)
            {
                throw new ArgumentException("RowKey attribute duplication.");
            }

            if (valueAccessor.Type != _stringType)
            {
                throw new ArgumentException("RowKey must have a string type.");
            }

            _rowKey = valueAccessor;

            _nameChanges.Add(valueAccessor.Name, "RowKey");
        }

        /// <summary>
        ///     Processes timestamp accessor.
        /// </summary>
        /// <param name="valueAccessor">Timestamp accessor.</param>
        private void ProcessTimestamp(IValueAccessor<T> valueAccessor)
        {
            if (_timestamp != null)
            {
                throw new ArgumentException("Timestamp attribute duplication.");
            }

            if (valueAccessor.Type != _datetimeType)
            {
                throw new ArgumentException("Timestamp must have a DateTime type.");
            }

            _timestamp = valueAccessor;
        }

        /// <summary>
        ///     Processes ETag accessor.
        /// </summary>
        /// <param name="valueAccessor">Timestamp accessor.</param>
        private void ProcessETag(IValueAccessor<T> valueAccessor)
        {
            if (_timestamp != null)
            {
                throw new ArgumentException("ETag attribute duplication.");
            }

            if (valueAccessor.Type != _stringType)
            {
                throw new ArgumentException("ETag must have a String type.");
            }

            _etag = valueAccessor;
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

                // Checks member for a partition key attribute
                if (Attribute.IsDefined(memberInfo, _partitionKeyAttributeType, false))
                {
                    ProcessPartitionKey(valueAccessor);
                    continue;
                }

                // Checks member for a row key attribute
                if (Attribute.IsDefined(memberInfo, _rowKeyAttributeType, false))
                {
                    ProcessRowKey(valueAccessor);
                    continue;
                }

                // Checks member for a timestamp attribute
                if (Attribute.IsDefined(memberInfo, _timestampAttributeType, false))
                {
                    ProcessTimestamp(valueAccessor);
                    continue;
                }

                // Checks member for an etag attribute
                if (Attribute.IsDefined(memberInfo, _etagAttributeType, false))
                {
                    ProcessETag(valueAccessor);
                    continue;
                }

                // Keep as a regular property
                _memberAccessors.Add(valueAccessor);
            }

            // Checks for a key attributes
            if (_partitionKey == null && _rowKey == null)
            {
                throw new ArgumentException("PartitionKey or RowKey attribute must be defined.");
            }
        }
    }
}