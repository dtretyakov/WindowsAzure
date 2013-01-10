using System;
using System.Collections.Generic;
using System.Reflection;
using WindowsAzure.Table.Attributes;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Keeps en entity type data.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public sealed class EntityTypeData<T> : IEntityTypeData<T>
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

        private readonly Type _datetimeType = typeof (DateTime);
        private readonly Dictionary<String, String> _nameChanges;
        private readonly Type _partitionKeyAttributeType = typeof (PartitionKeyAttribute);
        private readonly List<IValueAccessor<T>> _properties;
        private readonly Type _rowKeyAttributeType = typeof (RowKeyAttribute);
        private readonly Type _stringType = typeof (String);
        private readonly Type _timestampAttributeType = typeof (TimestampAttribute);
        private readonly Type _etagAttributeType = typeof(ETagAttribute);
        private IValueAccessor<T> _partitionKey;
        private IValueAccessor<T> _rowKey;
        private IValueAccessor<T> _timestamp;
        private IValueAccessor<T> _etag;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public EntityTypeData()
        {
            Type entityType = typeof (T);

            _nameChanges = new Dictionary<string, string>();
            _properties = new List<IValueAccessor<T>>();

            var typeMembers = new List<MemberInfo>(entityType.GetProperties(Flags));
            typeMembers.AddRange(entityType.GetFields(Flags));

            ProcessMembers(typeMembers);
        }

        /// <summary>
        ///     Gets a partiton key accessor.
        /// </summary>
        public IValueAccessor<T> PartitionKey
        {
            get { return _partitionKey; }
        }

        /// <summary>
        ///     Gets a row key accessor.
        /// </summary>
        public IValueAccessor<T> RowKey
        {
            get { return _rowKey; }
        }

        /// <summary>
        ///     Gets a timestamp accessor.
        /// </summary>
        public IValueAccessor<T> Timestamp
        {
            get { return _timestamp; }
        }

        /// <summary>
        /// Gets an etag accessor.
        /// </summary>
        public IValueAccessor<T> ETag { get { return _etag; } }

        /// <summary>
        ///     Gets a properties value accessors.
        /// </summary>
        public IEnumerable<IValueAccessor<T>> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        ///     Gets an entity partition & row key name changes.
        /// </summary>
        public IDictionary<string, string> NameChanges
        {
            get { return _nameChanges; }
        }

        /// <summary>
        /// Processes partition key accessor.
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
        /// Processes row ket accessor.
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
        /// Processes timestamp accessor.
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
        /// Processes ETag accessor.
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
        /// Processes type members.
        /// </summary>
        /// <param name="memberInfos">Type memebers.</param>
        private void ProcessMembers(IEnumerable<MemberInfo> memberInfos)
        {
            foreach (MemberInfo memberInfo in memberInfos)
            {
                IValueAccessor<T> valueAccessor = ValueAccessorFactory.Create<T>(memberInfo);

                // Checks property for a partition key attribute
                if (Attribute.IsDefined(memberInfo, _partitionKeyAttributeType, false))
                {
                    ProcessPartitionKey(valueAccessor);
                    continue;
                }

                // Checks property for a row key attribute
                if (Attribute.IsDefined(memberInfo, _rowKeyAttributeType, false))
                {
                    ProcessRowKey(valueAccessor);
                    continue;
                }

                // Checks property for a timestamp attribute
                if (Attribute.IsDefined(memberInfo, _timestampAttributeType, false))
                {
                    ProcessTimestamp(valueAccessor);
                    continue;
                }

                // Checks property for an etag attribute
                if (Attribute.IsDefined(memberInfo, _etagAttributeType, false))
                {
                    ProcessETag(valueAccessor);
                    continue;
                }

                // Keep as a regular property
                _properties.Add(valueAccessor);
            }

            if (_partitionKey == null)
            {
                throw new ArgumentException("PartitionKey attribute must be defined.");
            }

            if (_rowKey == null)
            {
                throw new ArgumentException("RowKey attribute must be defined.");
            }
        }
    }
}