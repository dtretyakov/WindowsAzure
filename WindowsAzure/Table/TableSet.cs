using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Table.Queryable;
using WindowsAzure.Table.Queryable.Base;
using WindowsAzure.Table.RequestExecutor;
using WindowsAzure.Table.Wrappers;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table
{
    /// <summary>
    ///     Windows Azure Table entity set.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public sealed class TableSet<TEntity> : Query<TEntity>, ITableSet<TEntity> where TEntity : class, new()
    {
        private readonly string _tableName;
        private readonly CloudTable _cloudTable;
        internal readonly TableRequestExecutorFactory<TEntity> RequestExecutorFactory;
        private ExecutionMode _executionMode = ExecutionMode.Sequential;
        internal ITableRequestExecutor<TEntity> RequestExecutor;

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
        public TableSet(
            CloudTableClient cloudTableClient, 
            string tableName)
        {
            if (cloudTableClient == null)
            {
                throw new ArgumentNullException(nameof(cloudTableClient));
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            _tableName = tableName;
            _cloudTable = cloudTableClient.GetTableReference(tableName);
            var cloudTableWrapper = new CloudTableWrapper(_cloudTable);
            var entityConverter = new TableEntityConverter<TEntity>();

            RequestExecutorFactory = new TableRequestExecutorFactory<TEntity>(cloudTableWrapper, entityConverter);
            Provider = new TableQueryProvider<TEntity>(cloudTableWrapper, entityConverter);
            RequestExecutor = RequestExecutorFactory.Create(_executionMode);
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
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.Execute(entity, TableOperation.Insert);
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
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.ExecuteAsync(entity, TableOperation.Insert, cancellationToken);
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
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatches(entities, TableOperation.Insert);
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
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatchesAsync(entities, TableOperation.Insert, cancellationToken);
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
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.Execute(entity, TableOperation.InsertOrReplace);
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
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.ExecuteAsync(entity, TableOperation.InsertOrReplace, cancellationToken);
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
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatches(entities, TableOperation.InsertOrReplace);
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
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatchesAsync(entities, TableOperation.InsertOrReplace, cancellationToken);
        }

        /// <summary>
        ///     Inserts or merges an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        public TEntity AddOrMerge(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.Execute(entity, TableOperation.InsertOrMerge);
        }

        /// <summary>
        ///     Inserts or merges an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        public Task<TEntity> AddOrMergeAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.ExecuteAsync(entity, TableOperation.InsertOrMerge, cancellationToken);
        }

        /// <summary>
        ///     Inserts or merges an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        public IEnumerable<TEntity> AddOrMerge(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatches(entities, TableOperation.InsertOrMerge);
        }

        /// <summary>
        ///     Inserts or merges an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entities.</returns>
        public Task<IEnumerable<TEntity>> AddOrMergeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatchesAsync(entities, TableOperation.InsertOrMerge, cancellationToken);
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
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.Execute(entity, TableOperation.Replace);
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
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.ExecuteAsync(entity, TableOperation.Replace, cancellationToken);
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
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatches(entities, TableOperation.Replace);
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
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatchesAsync(entities, TableOperation.Replace, cancellationToken);
        }

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        public void Remove(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            RequestExecutor.ExecuteWithoutResult(entity, TableOperation.Delete);
        }

        /// <summary>
        ///     Removes an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken))
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return RequestExecutor.ExecuteWithoutResultAsync(entity, TableOperation.Delete, cancellationToken);
        }

        /// <summary>
        ///     Removes an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        public void Remove(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            RequestExecutor.ExecuteBatchesWithoutResult(entities, TableOperation.Delete);
        }

        /// <summary>
        ///     Removes an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            return RequestExecutor.ExecuteBatchesWithoutResultAsync(entities, TableOperation.Delete, cancellationToken);
        }

        /// <summary>
        ///     Gets the table name.
        /// </summary>
        public string Name
        {
            get { return _tableName; }
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

                RequestExecutor = RequestExecutorFactory.Create(_executionMode);
            }
        }

        /// <summary>
        ///     Creates the table if it does not already exist.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if table was created; otherwise, <c>false</c>.
        /// </returns>
        public bool CreateIfNotExists()
        {
            return _cloudTable.CreateIfNotExists();
        }

        /// <summary>
        ///     Initiates an asynchronous operation to create a table if it does not already exist.
        /// </summary>
        /// <param name="cancellationToken">
        ///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for a
        ///     task to complete.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Threading.Tasks.Task`1" /> object of type <c>bool</c> that represents the asynchronous
        ///     operation.
        /// </returns>
        public Task<bool> CreateIfNotExistsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return _cloudTable.CreateIfNotExistsAsync(null, null, cancellationToken);
        }
    }
}