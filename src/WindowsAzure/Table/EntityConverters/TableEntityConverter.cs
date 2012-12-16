using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Handles an entities conversions.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public class TableEntityConverter<TEntity> : ITableEntityConverter<TEntity> where TEntity : new()
    {
        private readonly Type _entityType;
        private readonly Dictionary<String, String> _nameMappings;
        private readonly Type _partitionKeyAttributeType = typeof (PartitionKeyAttribute);
        private readonly Dictionary<String, PropertyInfo> _properties;
        private readonly Type _rowKeyAttributeType = typeof (RowKeyAttribute);
        private readonly Type _stringType = typeof (String);

        private PropertyInfo _partitionKeyProperty;
        private PropertyInfo _rowKeyProperty;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public TableEntityConverter()
        {
            _nameMappings = new Dictionary<string, string>();
            _properties = new Dictionary<String, PropertyInfo>();
            _entityType = typeof (TEntity);

            IntializeProperties();
        }

        /// <summary>
        ///     Gets an entity property name maping connection.
        /// </summary>
        public IDictionary<string, string> NameMappings
        {
            get { return _nameMappings; }
        }

        /// <summary>
        ///     Creates an ITableEntity by TEntity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Table entity.</returns>
        public ITableEntity GetEntity(TEntity entity)
        {
            var result = new DynamicTableEntity
                             {
                                 PartitionKey = _partitionKeyProperty.GetStringValue(entity),
                                 RowKey = _rowKeyProperty.GetStringValue(entity),
                                 ETag = "*"
                             };

            foreach (var property in _properties)
            {
                result.Properties.Add(property.Key, property.Value.GetEntityProperty(entity));
            }

            return result;
        }

        /// <summary>
        ///     Creates a TEntity by DynamicTableEntity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>Entity.</returns>
        public TEntity GetEntity(DynamicTableEntity tableEntity)
        {
            var result = new TEntity();

            _partitionKeyProperty.SetValue(result, tableEntity.PartitionKey);
            _rowKeyProperty.SetValue(result, tableEntity.RowKey);

            foreach (var property in _properties)
            {
                EntityProperty entityProperty;

                if (tableEntity.Properties.TryGetValue(property.Key, out entityProperty))
                {
                    property.Value.SetPropertyValue(entityProperty, result);
                }
            }

            return result;
        }

        /// <summary>
        ///     Retrieves type properties.
        /// </summary>
        private void IntializeProperties()
        {
            foreach (PropertyInfo property in _entityType.GetProperties())
            {
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

                    _nameMappings.Add(property.Name, "PartitionKey");

                    continue;
                }

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

                    _nameMappings.Add(property.Name, "RowKey");

                    continue;
                }

                _properties.Add(property.Name, property);
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
    }
}