using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table.EntityFormatters;
using GitHub.WindowsAzure.Table.Queryable;
using GitHub.WindowsAzure.Table.Queryable.Base;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Table
{
    /// <summary>
    ///     Windows Azure Table entity set.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public sealed class TableSet<TEntity> : Query<TEntity> where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityFormatter<TEntity> _formatter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        public TableSet(CloudTableClient cloudTableClient)
            : this(cloudTableClient, new TableEntityFormatter<TEntity>())
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="formatter">Entities formatter.</param>
        public TableSet(CloudTableClient cloudTableClient, ITableEntityFormatter<TEntity> formatter)
            : this(cloudTableClient, typeof (TEntity).Name, formatter)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="formatter">Entities formatter.</param>
        public TableSet(CloudTableClient cloudTableClient, string tableName,
                        ITableEntityFormatter<TEntity> formatter)
            : this(cloudTableClient, tableName, formatter,
                   new TableQueryProvider<TEntity>(cloudTableClient, tableName, formatter))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="formatter">Entities formatter.</param>
        /// <param name="queryProvider">Query provider.</param>
        public TableSet(CloudTableClient cloudTableClient, string tableName,
                        ITableEntityFormatter<TEntity> formatter, IQueryProvider queryProvider)
            : base(queryProvider)
        {
            _formatter = formatter;
            _cloudTable = cloudTableClient.GetTableReference(tableName);
            _cloudTable.CreateIfNotExists();
        }

        /// <summary>
        ///     Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            ITableEntity tableEntity = _formatter.GetEntity(entity);

            _cloudTable.Execute(TableOperation.Insert(tableEntity));

            return entity;
        }

        /// <summary>
        ///     Inserts a new entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        public async Task<List<TEntity>> AddAsync(List<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _formatter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Insert(tableEntity));
            }

            _cloudTable.ExecuteBatch(tableBatchOperation);

            return new List<TEntity>(entities);
        }

        /// <summary>
        ///     Updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Updated entity.</returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            ITableEntity tableEntity = _formatter.GetEntity(entity);

            _cloudTable.Execute(TableOperation.Merge(tableEntity));

            return entity;
        }

        /// <summary>
        ///     Updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Updated entities.</returns>
        public async Task<List<TEntity>> UpdateAsync(List<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _formatter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Merge(tableEntity));
            }

            _cloudTable.ExecuteBatch(tableBatchOperation);

            return new List<TEntity>(entities);
        }

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Result.</returns>
        public async Task RemoveAsync(TEntity entity)
        {
            ITableEntity tableEntity = _formatter.GetEntity(entity);

            _cloudTable.Execute(TableOperation.Delete(tableEntity));
        }

        /// <summary>
        ///     Removes an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Result.</returns>
        public async Task RemoveAsync(List<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _formatter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Delete(tableEntity));
            }

            _cloudTable.ExecuteBatch(tableBatchOperation);
        }
    }
}