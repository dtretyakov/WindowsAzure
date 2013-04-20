using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Table.Queryable.Base;

namespace WindowsAzure.Table
{
    /// <summary>
    ///     Windows Azure Table entity set.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public sealed class TableSet<TEntity> : Query<TEntity>, ITableSet<TEntity> where TEntity : class, new()
    {
        private readonly CloudTable _cloudTable;
        private readonly TableEntityConverter<TEntity> _entityConverter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        public TableSet(CloudTableClient cloudTableClient)
            : this(cloudTableClient, typeof (TEntity).Name)
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        public TableSet(CloudTableClient cloudTableClient, string tableName)
        {
            if (cloudTableClient == null)
            {
                throw new ArgumentNullException("cloudTableClient");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            _cloudTable = cloudTableClient.GetTableReference(tableName);
            _entityConverter = new TableEntityConverter<TEntity>();

            Provider = new TableQueryProvider<TEntity>(_cloudTable, _entityConverter);
            Configuration = new TableSetConfiguration();
        }

        /// <summary>
        ///     Gets an <see cref="TableSet{TEntity}" /> configuration.
        /// </summary>
        public TableSetConfiguration Configuration { get; private set; }

        /// <summary>
        ///     Inserts a new entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            return _cloudTable.ExecuteAsync(
                TableOperation.Insert(tableEntity), cancellationToken)
                              .Then(result =>
                                  {
                                      var value = (DynamicTableEntity) result.Result;
                                      return _entityConverter.GetEntity(value);
                                  }, cancellationToken);
        }

        /// <summary>
        ///     Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public TEntity Add(TEntity entity)
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            TableResult result = _cloudTable.Execute(TableOperation.Insert(tableEntity));

            return _entityConverter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Inserts a new entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entities.</returns>
        public Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Insert(tableEntity));
            }

            return _cloudTable
                .ExecuteBatchAsync(tableBatchOperation, cancellationToken)
                .Then(result => result.Select(p =>
                    {
                        var value = (DynamicTableEntity) p.Result;
                        return _entityConverter.GetEntity(value);
                    }), cancellationToken);
        }

        /// <summary>
        ///     Inserts a new entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        public IEnumerable<TEntity> Add(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Insert(tableEntity));
            }

            IList<TableResult> result = _cloudTable.ExecuteBatch(tableBatchOperation);

            return result.Select(p => _entityConverter.GetEntity((DynamicTableEntity) p.Result)).AsEnumerable();
        }

        /// <summary>
        ///     Inserts or updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        public Task<TEntity> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            return _cloudTable.ExecuteAsync(TableOperation.InsertOrReplace(tableEntity), cancellationToken)
                              .Then(result =>
                                  {
                                      var value = (DynamicTableEntity) result.Result;
                                      return _entityConverter.GetEntity(value);
                                  }, cancellationToken);
        }

        /// <summary>
        ///     Inserts or updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public TEntity AddOrUpdate(TEntity entity)
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            TableResult result = _cloudTable.Execute(TableOperation.InsertOrReplace(tableEntity));

            return _entityConverter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Inserts or updates an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entities.</returns>
        public Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.InsertOrReplace(tableEntity));
            }

            return _cloudTable
                .ExecuteBatchAsync(tableBatchOperation, cancellationToken)
                .Then(result => result.Select(p =>
                    {
                        var value = (DynamicTableEntity) p.Result;
                        return _entityConverter.GetEntity(value);
                    }), cancellationToken);
        }

        /// <summary>
        ///     Inserts or updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        public IEnumerable<TEntity> AddOrUpdate(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.InsertOrReplace(tableEntity));
            }

            IList<TableResult> result = _cloudTable.ExecuteBatch(tableBatchOperation);

            return result.Select(p => _entityConverter.GetEntity((DynamicTableEntity) p.Result)).AsEnumerable();
        }

        /// <summary>
        ///     Updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated entity.</returns>
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            return _cloudTable.ExecuteAsync(TableOperation.Replace(tableEntity), cancellationToken)
                              .Then(result =>
                                  {
                                      var value = (DynamicTableEntity) result.Result;
                                      return _entityConverter.GetEntity(value);
                                  }, cancellationToken);
        }

        /// <summary>
        ///     Updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Updated entity.</returns>
        public TEntity Update(TEntity entity)
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            TableResult result = _cloudTable.Execute(TableOperation.Replace(tableEntity));

            return _entityConverter.GetEntity((DynamicTableEntity) result.Result);
        }

        /// <summary>
        ///     Updates an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated entities.</returns>
        public Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Replace(tableEntity));
            }

            return _cloudTable.ExecuteBatchAsync(tableBatchOperation, cancellationToken)
                              .Then(result => result.Select(
                                  p =>
                                      {
                                          var value = (DynamicTableEntity) p.Result;
                                          return _entityConverter.GetEntity(value);
                                      }), cancellationToken);
        }

        /// <summary>
        ///     Updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Updated entities.</returns>
        public IEnumerable<TEntity> Update(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Replace(tableEntity));
            }

            IList<TableResult> result = _cloudTable.ExecuteBatch(tableBatchOperation);

            return result.Select(p => _entityConverter.GetEntity((DynamicTableEntity) p.Result)).AsEnumerable();
        }

        /// <summary>
        ///     Removes an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result.</returns>
        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            return _cloudTable.ExecuteAsync(TableOperation.Delete(tableEntity), cancellationToken);
        }

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Result.</returns>
        public void Remove(TEntity entity)
        {
            ITableEntity tableEntity = _entityConverter.GetEntity(entity);

            _cloudTable.Execute(TableOperation.Delete(tableEntity));
        }

        /// <summary>
        ///     Removes an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result.</returns>
        public Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Delete(tableEntity));
            }

            return _cloudTable.ExecuteBatchAsync(tableBatchOperation, cancellationToken);
        }

        /// <summary>
        ///     Removes an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Result.</returns>
        public void Remove(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            var tableBatchOperation = new TableBatchOperation();

            foreach (TEntity entity in entities)
            {
                ITableEntity tableEntity = _entityConverter.GetEntity(entity);
                tableBatchOperation.Add(TableOperation.Delete(tableEntity));
            }

            _cloudTable.ExecuteBatch(tableBatchOperation);
        }
    }
}