using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Table entity converter.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface ITableEntityConverter<TEntity> where TEntity : new()
    {
        /// <summary>
        ///     Gets an entity property name maping connection.
        /// </summary>
        IDictionary<String, String> NameChanges { get; }

        /// <summary>
        ///     Converts an entity to table entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Table entity.</returns>
        ITableEntity GetEntity(TEntity entity);

        /// <summary>
        ///     Converts a table entity to entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>Entity.</returns>
        TEntity GetEntity(DynamicTableEntity tableEntity);
    }
}