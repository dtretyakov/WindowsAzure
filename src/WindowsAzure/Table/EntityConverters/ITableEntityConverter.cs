using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Table.EntityConverters
{
    /// <summary>
    /// Table entity converter.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface ITableEntityConverter<TEntity> where TEntity : new()
    {
        ITableEntity GetEntity(TEntity entity);

        TEntity GetEntity(DynamicTableEntity tableEntity);
    }
}