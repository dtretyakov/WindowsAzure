using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WindowsAzure.Table.Queryable;

namespace WindowsAzure.Table.Extensions
{
    /// <summary>
    ///     LINQ extensions for a asynchronous execution.
    /// </summary>
    public static class AsyncQueryableExtensions
    {
        public static Task<List<T>> ToListAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.ToList());
            }

            return tableQueryProvider.ExecuteAsync(source.Expression)
                                     .Then(result => ((IEnumerable<T>)result).ToList());
        }

        public static Task<IEnumerable<T>> TakeAsync<T>(this IQueryable<T> source, int count) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.Take(count).AsEnumerable());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(count).Expression)
                                     .Then(result => (IEnumerable<T>) result);
        }

        public static Task<T> FirstAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.First());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(1).Expression)
                                     .Then(result => ((IEnumerable<T>) result).First());
        }

        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.FirstOrDefault());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(1).Expression)
                                     .Then(result => ((IEnumerable<T>) result).FirstOrDefault());
        }

        public static Task<T> SingleAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.Single());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(2).Expression)
                                     .Then(result => ((IEnumerable<T>) result).Single());
        }

        public static Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return TaskHelpers.FromResult(source.SingleOrDefault());
            }

            return tableQueryProvider.ExecuteAsync(source.Take(2).Expression)
                                     .Then(result => ((IEnumerable<T>) result).SingleOrDefault());
        }
    }
}