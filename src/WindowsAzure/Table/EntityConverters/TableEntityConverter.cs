using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.Infrastructure;
using WindowsAzure.Table.EntityConverters.TypeData;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Handles an entities conversions.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class TableEntityConverter<T> : ITableEntityConverter<T> where T : new()
    {
        private readonly IEntityTypeData<T> _typeData;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public TableEntityConverter()
        {
            _typeData = EntityTypeDataFactory.GetEntityTypeData<T>();
        }

        /// <summary>
        ///     Gets an entity property name maping connection.
        /// </summary>
        public IDictionary<string, string> NameChanges
        {
            get { return _typeData.NameChanges; }
        }

        /// <summary>
        ///     Creates an ITableEntity by TEntity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Table entity.</returns>
        public ITableEntity GetEntity(T entity)
        {
            var result = new DynamicTableEntity
                             {
                                 PartitionKey = (String) _typeData.PartitionKey.GetValue(entity),
                                 RowKey = (String) _typeData.RowKey.GetValue(entity),
                                 ETag = _typeData.ETag != null ? (string) _typeData.ETag.GetValue(entity) : "*"
                             };

            foreach (var property in _typeData.Properties)
            {
                result.Properties.Add(property.Name, property.Type.GetEntityProperty(property.GetValue(entity)));
            }

            return result;
        }

        /// <summary>
        ///     Creates a TEntity by DynamicTableEntity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>Entity.</returns>
        public T GetEntity(DynamicTableEntity tableEntity)
        {
            if (tableEntity == null)
            {
                throw new ArgumentNullException("tableEntity");
            }

            var result = new T();

            _typeData.PartitionKey.SetValue(result, tableEntity.PartitionKey);
            _typeData.RowKey.SetValue(result, tableEntity.RowKey);

            if (_typeData.ETag != null)
            {
                _typeData.ETag.SetValue(result, tableEntity.ETag);
            }

            if (_typeData.Timestamp != null)
            {
                _typeData.Timestamp.SetValue(result, tableEntity.Timestamp);
            }

            foreach (var property in _typeData.Properties)
            {
                EntityProperty entityProperty;

                if (tableEntity.Properties.TryGetValue(property.Name, out entityProperty))
                {
                    property.SetValue(result, entityProperty.GetValue());
                }
            }

            return result;
        }
    }
}