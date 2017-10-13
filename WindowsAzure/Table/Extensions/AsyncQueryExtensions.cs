using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WindowsAzure.Table.Queryable;

namespace WindowsAzure.Table.Extensions
{
    /// <summary>
    ///     LINQ extensions for a asynchronous query execution.
    /// </summary>
    public static class AsyncQueryExtensions
    {
        /// <summary>
        ///     Executes a query ToList method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of entities.</returns>
        public static Task<List<T>> ToListAsync<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.ToList());
            }

            return tableQueryProvider.ExecuteAsync(source.Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>) result).ToList(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query ToList method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="predicate">Predicate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of entities.</returns>
        public static Task<List<T>> ToListAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.ToList());
            }

            return tableQueryProvider.ExecuteAsync(source.Where(predicate).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).ToList(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query Take method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="count">Entities count.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entities.</returns>
        public static Task<List<T>> TakeAsync<T>(
            this IQueryable<T> source,
            int count,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.Take(count).ToList());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(count).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).ToList(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query First method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> FirstAsync<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.First());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(1).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>) result).First(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query First method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="predicate">Predicate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> FirstAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T,bool>> predicate,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.First());
            }

            return tableQueryProvider.ExecuteAsync(source.Where(predicate).Take(1).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).First(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query FirstOrDefault asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> FirstOrDefaultAsync<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.FirstOrDefault());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(1).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).FirstOrDefault(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query FirstOrDefault asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="predicate">Predicate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> FirstOrDefaultAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.FirstOrDefault());
            }

            return tableQueryProvider.ExecuteAsync(source.Where(predicate).Take(1).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).FirstOrDefault(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query Single asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> SingleAsync<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.Single());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(2).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>) result).Single(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query Single asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="predicate">Predicate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> SingleAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.Single());
            }

            return tableQueryProvider.ExecuteAsync(source.Where(predicate).Take(2).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).Single(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query SingleOrDefault method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> SingleOrDefaultAsync<T>(
            this IQueryable<T> source,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.SingleOrDefault());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(2).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>) result).SingleOrDefault(), cancellationToken);
        }

        /// <summary>
        ///     Executes a query SingleOrDefault method asynchronously.
        /// </summary>
        /// <typeparam name="T">The entity type of the query.</typeparam>
        /// <param name="source">Query.</param>
        /// <param name="predicate">Predicate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Entity.</returns>
        public static Task<T> SingleOrDefaultAsync<T>(
            this IQueryable<T> source,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default (CancellationToken))
        {
            var tableQueryProvider = source.Provider as IAsyncQueryProvider;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.SingleOrDefault());
            }

            return tableQueryProvider.ExecuteAsync(source.Where(predicate).Take(2).Expression, cancellationToken)
                                     .Then(result => ((IEnumerable<T>)result).SingleOrDefault(), cancellationToken);
        }
    }
}