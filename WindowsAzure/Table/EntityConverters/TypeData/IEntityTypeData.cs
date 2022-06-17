using System.Collections.Generic;
#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#elif AZUREDATATABLES
using Azure.Data.Tables;
using DynamicTableEntity = Azure.Data.Tables.TableEntity;
#else
using Microsoft.Azure.Cosmos.Table;
#endif

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Defines base interface for an entity type data converter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal interface IEntityTypeData<T>
    {
        /// <summary>
        ///     Gets an entity members name changes.
        /// </summary>
        IDictionary<string, string> NameChanges { get; }

        /// <summary>
        ///     Converts DynamicTableEntity into POCO entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <returns>POCO entity.</returns>
        T GetEntity(DynamicTableEntity tableEntity);

        /// <summary>
        ///     Converts POCO entity into ITableEntity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <returns>Table entity.</returns>
        ITableEntity GetEntity(T entity);
    }
}