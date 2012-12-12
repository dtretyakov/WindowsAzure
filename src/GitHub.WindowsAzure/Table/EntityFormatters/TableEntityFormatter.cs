using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GitHub.WindowsAzure.Table.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Table.EntityFormatters
{
    public class TableEntityFormatter<TEntity> : ITableEntityFormatter<TEntity> where TEntity : new()
    {
        private const string PartitionKey = "PartitionKey";
        private const string RowKey = "RowKey";
        private readonly Dictionary<string, PropertyInfo> _properties;
        private readonly Type _stringType = typeof (String);
        private PropertyInfo _partitionKeyProperty;
        private PropertyInfo _rowKeyProperty;

        public TableEntityFormatter()
        {
            _properties = new Dictionary<String, PropertyInfo>();

            IntializeProperties();
        }

        public ITableEntity GetEntity(TEntity entity)
        {
            var result = new DynamicTableEntity();

            result.Properties.Add(PartitionKey, _partitionKeyProperty.GetStringValue(entity));
            result.Properties.Add(RowKey, _rowKeyProperty.GetStringValue(entity));

            foreach (var property in _properties)
            {
                result.Properties.Add(property.Key, property.Value.GetEntityProperty(entity));
            }

            return result;
        }


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

        private void IntializeProperties()
        {
            foreach (PropertyInfo property in typeof (TEntity).GetProperties())
            {
                Attribute[] attributes = Attribute.GetCustomAttributes(property, typeof (TableKeyAttribute));
                TableKeyAttribute tableKeyAttribute = attributes.Cast<TableKeyAttribute>().SingleOrDefault();

                if (tableKeyAttribute is PartitionKeyAttribute)
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
                }
                else if (tableKeyAttribute is RowKeyAttribute)
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
                }
                else
                {
                    _properties.Add(property.Name, property);
                }
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