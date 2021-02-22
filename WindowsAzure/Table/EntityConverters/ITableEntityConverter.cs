using System.Collections.Generic;
#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#else
using Microsoft.Azure.Cosmos.Table;
#endif

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Table entity converter.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface ITableEntityConverter<TEntity> where TEntity : new()
    {
        /// <summary>
        ///     Gets an entity property name mapping connection.
        /// </summary>
        IDictionary<string, string> NameChanges { get; }

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