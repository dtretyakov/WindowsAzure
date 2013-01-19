using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.KeyProperties
{
    /// <summary>
    ///     Handles access to the key property.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IKeyProperty<T>
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
        ///     Checks whether memberInfo contains an specific criteria.
        /// </summary>
        /// <param name="memberInfo">Member info.</param>
        /// <param name="accessor">Value accessor.</param>
        /// <returns>Result.</returns>
        bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor);

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