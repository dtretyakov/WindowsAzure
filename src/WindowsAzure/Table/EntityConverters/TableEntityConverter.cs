using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.Infrastructure;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Handles an entities conversions.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public class TableEntityConverter<TEntity> : TableEntityConverterBase,
                                                 ITableEntityConverter<TEntity> where TEntity : new()
    {
        private readonly EntityTypeData _typeData;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public TableEntityConverter()
        {
            _typeData = GetEntityTypeData(typeof (TEntity));
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
        public ITableEntity GetEntity(TEntity entity)
        {
            var result = new DynamicTableEntity
                             {
                                 PartitionKey = _typeData.PartitionKey.GetStringValue(entity),
                                 RowKey = _typeData.RowKey.GetStringValue(entity),
                                 ETag = "*"
                             };

            foreach (PropertyInfo property in _typeData.Properties)
            {
                result.Properties.Add(property.Name, property.GetEntityProperty(entity));
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
            if (tableEntity == null)
            {
                throw new ArgumentNullException("tableEntity");
            }

            var result = new TEntity();

            _typeData.PartitionKey.SetValue(result, tableEntity.PartitionKey);
            _typeData.RowKey.SetValue(result, tableEntity.RowKey);

            foreach (PropertyInfo property in _typeData.Properties)
            {
                EntityProperty entityProperty;

                if (tableEntity.Properties.TryGetValue(property.Name, out entityProperty))
                {
                    property.SetPropertyValue(entityProperty, result);
                }
            }

            return result;
        }

        /// <summary>
        ///     Retrieves type properties.
        /// </summary>
        private EntityTypeData GetEntityTypeData(Type entityType)
        {
            return TypesData.GetOrAdd(entityType, type => new EntityTypeData(entityType));
        }
    }
}