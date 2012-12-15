using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table.EntityConverters;
using GitHub.WindowsAzure.Table.Extensions;
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
        private readonly ITableEntityConverter<TEntity> _converter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        public TableSet(CloudTableClient cloudTableClient)
            : this(cloudTableClient, new TableEntityConverter<TEntity>())
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        public TableSet(CloudTableClient cloudTableClient, string tableName)
            : this(cloudTableClient, tableName, new TableEntityConverter<TEntity>())
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="converter">Entities converter.</param>
        public TableSet(CloudTableClient cloudTableClient, ITableEntityConverter<TEntity> converter)
            : this(cloudTableClient, typeof (TEntity).Name, converter)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="converter">Entities converter.</param>
        public TableSet(CloudTableClient cloudTableClient, string tableName,
                        ITableEntityConverter<TEntity> converter)
            : this(cloudTableClient, tableName, converter,
                   new TableQueryProvider<TEntity>(cloudTableClient, tableName, converter))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="converter">Entities converter.</param>
        /// <param name="queryProvider">Query provider.</param>
        public TableSet(CloudTableClient cloudTableClient, string tableName,
                        ITableEntityConverter<TEntity> converter, IQueryProvider queryProvider)
            : base(queryProvider)
        {
            _converter = converter;
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
            ITableEntity tableEntity = _converter.GetEntity(entity);

            TableResult result = await _cloudTable.ExecuteAsync(TableOperation.Insert(tableEntity));

            return _converter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public TEntity Add(TEntity entity)
        {
            ITableEntity tableEntity = _converter.GetEntity(entity);

            TableResult result = _cloudTable.Execute(TableOperation.Insert(tableEntity));

            return _converter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Inserts a new entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        public async Task<IReadOnlyList<TEntity>> AddAsync(IEnumerable<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _converter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Insert(tableEntity));
            }

            IList<TableResult> result = await _cloudTable.ExecuteBatchAsync(tableBatchOperation);

            return result.Select(p => _converter.GetEntity((DynamicTableEntity) p.Result)).ToList();
        }

        /// <summary>
        ///     Inserts a new entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        public IReadOnlyList<TEntity> Add(IEnumerable<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _converter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Insert(tableEntity));
            }

            IList<TableResult> result = _cloudTable.ExecuteBatch(tableBatchOperation);

            return result.Select(p => _converter.GetEntity((DynamicTableEntity) p.Result)).ToList();
        }

        /// <summary>
        ///     Updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Updated entity.</returns>
        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            ITableEntity tableEntity = _converter.GetEntity(entity);

            TableResult result = await _cloudTable.ExecuteAsync(TableOperation.Replace(tableEntity));

            return _converter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Updated entity.</returns>
        public TEntity Update(TEntity entity)
        {
            ITableEntity tableEntity = _converter.GetEntity(entity);

            TableResult result = _cloudTable.Execute(TableOperation.Replace(tableEntity));

            return _converter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Updated entities.</returns>
        public async Task<IReadOnlyList<TEntity>> UpdateAsync(IEnumerable<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _converter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Replace(tableEntity));
            }

            IList<TableResult> result = await _cloudTable.ExecuteBatchAsync(tableBatchOperation);

            return result.Select(p => _converter.GetEntity((DynamicTableEntity) p.Result)).ToList();
        }

        /// <summary>
        ///     Updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Updated entities.</returns>
        public IReadOnlyList<TEntity> Update(IEnumerable<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _converter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Replace(tableEntity));
            }

            IList<TableResult> result = _cloudTable.ExecuteBatch(tableBatchOperation);

            return result.Select(p => _converter.GetEntity((DynamicTableEntity) p.Result)).ToList();
        }

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Result.</returns>
        public async Task RemoveAsync(TEntity entity)
        {
            ITableEntity tableEntity = _converter.GetEntity(entity);

            await _cloudTable.ExecuteAsync(TableOperation.Delete(tableEntity));
        }

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Result.</returns>
        public void Remove(TEntity entity)
        {
            ITableEntity tableEntity = _converter.GetEntity(entity);

            _cloudTable.Execute(TableOperation.Delete(tableEntity));
        }

        /// <summary>
        ///     Removes an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Result.</returns>
        public async Task RemoveAsync(IEnumerable<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _converter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Delete(tableEntity));
            }

            await _cloudTable.ExecuteBatchAsync(tableBatchOperation);
        }

        /// <summary>
        ///     Removes an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Result.</returns>
        public void Remove(IEnumerable<TEntity> entities)
        {
            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _converter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Delete(tableEntity));
            }

            _cloudTable.ExecuteBatch(tableBatchOperation);
        }
    }
}