using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to the property value.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IProperty<in T>
    {
        /// <summary>
        ///     Sets a POCO member value from table entity.
        /// </summary>
        /// <param name="tableEntity">Table entity.</param>
        /// <param name="entity">POCO entity.</param>
        void SetMemberValue(DynamicTableEntity tableEntity, T entity);

        /// <summary>
        ///     Gets a POCO member value for table entity.
        /// </summary>
        /// <param name="entity">POCO entity.</param>
        /// <param name="tableEntity">Table entity.</param>
        void GetMemberValue(T entity, DynamicTableEntity tableEntity);
    }
}