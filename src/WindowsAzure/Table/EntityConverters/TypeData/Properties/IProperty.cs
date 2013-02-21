using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to property.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IProperty<in T>
    {
        /// <summary>
        ///     Gets a memeber names changes.
        /// </summary>
        IDictionary<string, string> NameChanges { get; }

        /// <summary>
        ///     Gets a value indication whether property has accessor.
        /// </summary>
        bool HasAccessor { get; }

        /// <summary>
        ///     Writes a table entity data to POCO entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <param name="entity">POCO entity.</param>
        void FillEntity(DynamicTableEntity tableEntity, T entity);

        /// <summary>
        ///     Writes a POCO entity data to table entity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <param name="tableEntity">Table entity.</param>
        void FillTableEntity(T entity, DynamicTableEntity tableEntity);
    }
}