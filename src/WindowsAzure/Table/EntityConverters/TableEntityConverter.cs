using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Handles an entities conversions.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class TableEntityConverter<T> : ITableEntityConverter<T> where T : class, new()
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
            return _typeData.GetEntity(entity);
        }

        /// <summary>
        ///     Creates a TEntity by DynamicTableEntity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>Entity.</returns>
        public T GetEntity(DynamicTableEntity tableEntity)
        {
            return _typeData.GetEntity(tableEntity);
        }
    }
}