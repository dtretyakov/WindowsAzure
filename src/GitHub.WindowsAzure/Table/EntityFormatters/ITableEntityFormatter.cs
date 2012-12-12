using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Table.EntityFormatters
{
    public interface ITableEntityFormatter<TEntity> where TEntity : new()
    {
        ITableEntity GetEntity(TEntity entity);

        TEntity GetEntity(DynamicTableEntity tableEntity);
    }
}