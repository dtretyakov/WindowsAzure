using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsAzure.Table
{
    /// <summary>
    ///     Interface for a Windows Azure TableSet.
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public interface ITableSet<TEntity> : IOrderedQueryable<TEntity> where TEntity : class, new()
    {
        /// <summary>
        ///     Gets or sets a value indicating request execution mode.
        /// </summary>
        ExecutionMode ExecutionMode { get; set; }

        /// <summary>
        ///     Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        TEntity Add(TEntity entity);

        /// <summary>
        ///     Inserts a new entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Inserts a new entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        IEnumerable<TEntity> Add(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Inserts a new entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entities.</returns>
        Task<IEnumerable<TEntity>> AddAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Inserts or updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        TEntity AddOrUpdate(TEntity entity);

        /// <summary>
        ///     Inserts or updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        Task<TEntity> AddOrUpdateAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Inserts or updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        IEnumerable<TEntity> AddOrUpdate(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Inserts or updates an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entities.</returns>
        Task<IEnumerable<TEntity>> AddOrUpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Inserts or merges an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Inserted entity.</returns>
        TEntity AddOrMerge(TEntity entity);

        /// <summary>
        ///     Inserts or merges an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entity.</returns>
        Task<TEntity> AddOrMergeAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Inserts or updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Inserted entities.</returns>
        IEnumerable<TEntity> AddOrMerge(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Inserts or updates an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Inserted entities.</returns>
        Task<IEnumerable<TEntity>> AddOrMergeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Updates an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Updated entity.</returns>
        TEntity Update(TEntity entity);

        /// <summary>
        ///     Updates an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated entity.</returns>
        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Updates an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Updated entities.</returns>
        IEnumerable<TEntity> Update(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Updates an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Updated entities.</returns>
        Task<IEnumerable<TEntity>> UpdateAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        ///     Removes an entity.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <returns>Result.</returns>
        void Remove(TEntity entity);

        /// <summary>
        ///     Removes an entity asynchronously.
        /// </summary>
        /// <param name="entity">Entity.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result.</returns>
        Task RemoveAsync(TEntity entity, CancellationToken cancellationToken = default (CancellationToken));

        /// <summary>
        ///     Removes an entities.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <returns>Result.</returns>
        void Remove(IEnumerable<TEntity> entities);

        /// <summary>
        ///     Removes an entities asynchronously.
        /// </summary>
        /// <param name="entities">Entities collection.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Result.</returns>
        Task RemoveAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default(CancellationToken));
    }
}