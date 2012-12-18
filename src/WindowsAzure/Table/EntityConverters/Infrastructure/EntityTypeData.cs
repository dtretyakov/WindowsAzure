using System;
using System.Collections.Generic;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.Infrastructure
{
    /// <summary>
    ///     Keeps en entity type data.
    /// </summary>
    public sealed class EntityTypeData
    {
        private readonly Dictionary<String, String> _nameChanges;
        private readonly Type _partitionKeyAttributeType = typeof (PartitionKeyAttribute);
        private readonly PropertyInfo _partitionKeyProperty;
        private readonly List<PropertyInfo> _properties;
        private readonly Type _rowKeyAttributeType = typeof (RowKeyAttribute);
        private readonly PropertyInfo _rowKeyProperty;
        private readonly Type _stringType = typeof (String);

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        public EntityTypeData(Type entityType)
        {
            if (entityType == null)
            {
                throw new ArgumentNullException("entityType");
            }

            _nameChanges = new Dictionary<string, string>();
            _properties = new List<PropertyInfo>();

            foreach (PropertyInfo property in entityType.GetProperties())
            {
                // Checks property for a partition key attribute
                if (Attribute.IsDefined(property, _partitionKeyAttributeType, false))
                {
                    if (_partitionKeyProperty != null)
                    {
                        throw new ArgumentException("PartitionKey attribute duplication.");
                    }

                    if (property.PropertyType != _stringType)
                    {
                        throw new ArgumentException("PartitionKey must be a string.");
                    }

                    _partitionKeyProperty = property;

                    _nameChanges.Add(property.Name, "PartitionKey");

                    continue;
                }

                // Checks property for a row key attribute
                if (Attribute.IsDefined(property, _rowKeyAttributeType, false))
                {
                    if (_rowKeyProperty != null)
                    {
                        throw new ArgumentException("RowKey attribute duplication.");
                    }

                    if (property.PropertyType != _stringType)
                    {
                        throw new ArgumentException("RowKey must be a string.");
                    }

                    _rowKeyProperty = property;

                    _nameChanges.Add(property.Name, "RowKey");

                    continue;
                }

                // Keep as a regular property
                _properties.Add(property);
            }

            if (_partitionKeyProperty == null)
            {
                throw new ArgumentException("PartitionKey attribute must be defined.");
            }

            if (_rowKeyProperty == null)
            {
                throw new ArgumentException("RowKey attribute must be defined.");
            }
        }

        /// <summary>
        ///     Gets a partiton key property info.
        /// </summary>
        public PropertyInfo PartitionKey
        {
            get { return _partitionKeyProperty; }
        }

        /// <summary>
        ///     Gets a row key property info.
        /// </summary>
        public PropertyInfo RowKey
        {
            get { return _rowKeyProperty; }
        }

        /// <summary>
        ///     Gets a properties infromation.
        /// </summary>
        public IEnumerable<PropertyInfo> Properties
        {
            get { return _properties; }
        }

        /// <summary>
        ///     Gets a entity properties name mappings to table entity properties.
        /// </summary>
        public IDictionary<string, string> NameChanges
        {
            get { return _nameChanges; }
        }
    }
}