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
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.ToList();
            }

            return ((IEnumerable<T>) await tableQueryProvider.ExecuteAsync(source.Expression)).ToList();
        }

        public static async Task<List<T>> TakeAsync<T>(this IQueryable<T> source, int count) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.Take(count).ToList();
            }

            return ((IEnumerable<T>) await tableQueryProvider.ExecuteAsync(source.Take(count).Expression)).ToList();
        }

        public static async Task<T> FirstAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.First();
            }

            return ((IEnumerable<T>) await tableQueryProvider.ExecuteAsync(source.Take(1).Expression)).First();
        }

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.FirstOrDefault();
            }

            return ((IEnumerable<T>) await tableQueryProvider.ExecuteAsync(source.Take(1).Expression)).FirstOrDefault();
        }

        public static async Task<T> SingleAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.Single();
            }

            return ((IEnumerable<T>) await tableQueryProvider.ExecuteAsync(source.Take(2).Expression)).Single();
        }

        public static async Task<T> SingleOrDefaultAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.SingleOrDefault();
            }

            return ((IEnumerable<T>) await tableQueryProvider.ExecuteAsync(source.Take(2).Expression)).SingleOrDefault();
        }
    }
}