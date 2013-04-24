using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.QueryExecutor;
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
        internal readonly TableQueryExecutorFactory<TEntity> QueryExecutorFactory;
        internal ITableQueryExecutor<TEntity> QueryExecutor;
        private ExecutionMode _executionMode = ExecutionMode.Sequential;

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

            CloudTable cloudTable = cloudTableClient.GetTableReference(tableName);
            var entityConverter = new TableEntityConverter<TEntity>();

            QueryExecutorFactory = new TableQueryExecutorFactory<TEntity>(cloudTable, entityConverter);
            Provider = new TableQueryProvider<TEntity>(cloudTable, entityConverter);
            QueryExecutor = QueryExecutorFactory.Create(_executionMode);
        }

        /// <summary>
        ///     Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public TEntity Add(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.Execute(entity, TableOperation.Insert);
        }

        /// <summary>
        ///     Inserts a new entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        public Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.ExecuteAsync(entity, TableOperation.Insert, cancellationToken);
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

            return QueryExecutor.ExecuteBatches(entities, TableOperation.Insert);
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

            return QueryExecutor.ExecuteBatchesAsync(entities, TableOperation.Insert, cancellationToken);
        }

        /// <summary>
        ///     Inserts or updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public TEntity AddOrUpdate(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.Execute(entity, TableOperation.InsertOrReplace);
        }

        /// <summary>
        ///     Inserts or updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        public Task<TEntity> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.ExecuteAsync(entity, TableOperation.InsertOrReplace, cancellationToken);
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

            return QueryExecutor.ExecuteBatches(entities, TableOperation.InsertOrReplace);
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

            return QueryExecutor.ExecuteBatchesAsync(entities, TableOperation.InsertOrReplace, cancellationToken);
        }

        /// <summary>
        ///     Updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Updated entity.</returns>
        public TEntity Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.Execute(entity, TableOperation.Replace);
        }

        /// <summary>
        ///     Updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated entity.</returns>
        public Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.ExecuteAsync(entity, TableOperation.Replace, cancellationToken);
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

            return QueryExecutor.ExecuteBatches(entities, TableOperation.Replace);
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

            return QueryExecutor.ExecuteBatchesAsync(entities, TableOperation.Replace, cancellationToken);
        }

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Result.</returns>
        public void Remove(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            QueryExecutor.Execute(entity, TableOperation.Delete);
        }


        /// <summary>
        ///     Removes an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result.</returns>
        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            return QueryExecutor.ExecuteAsync(entity, TableOperation.Delete, cancellationToken);
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

            QueryExecutor.ExecuteBatches(entities, TableOperation.Delete);
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

            return QueryExecutor.ExecuteBatchesAsync(entities, TableOperation.Delete, cancellationToken);
        }

        /// <summary>
        ///     Gets or sets a value indicating request execution mode.
        /// </summary>
        public ExecutionMode ExecutionMode
        {
            get { return _executionMode; }
            set
            {
                if (_executionMode == value)
                {
                    return;
                }

                _executionMode = value;

                QueryExecutor = QueryExecutorFactory.Create(_executionMode);
            }
        }
    }
}