using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace GitHub.WindowsAzure.Table.Queryable
{
    /// <summary>
    ///     Defines methods to create and asynchronously execute queries that are described by an
    ///     <see
    ///         cref="T:System.Linq.IQueryable" />
    ///     object.
    /// </summary>
    public interface IAsyncQueryProvider<TEntity> : IQueryProvider
    {
        /// <summary>
        ///     Asynchronously executes the query represented by a specified expression tree.
        /// </summary>
        /// <returns>
        ///     A Task containing the value that results from executing the specified query.
        /// </returns>
        /// <param name="expression">An expression tree that represents a LINQ query. </param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. </param>
        Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken);

        /// <summary>
        ///     Asynchronously executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <returns>
        ///     A Task containing the value that results from executing the specified query.
        /// </returns>
        /// <param name="expression">An expression tree that represents a LINQ query. </param>
        /// <typeparam name="TResult">The type of the value that results from executing the query. </typeparam>
        /// <param name="cancellationToken">The token to monitor for cancellation requests. </param>
        Task<TEntity> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken);
    }
}