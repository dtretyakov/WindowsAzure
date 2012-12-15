using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table.Queryable;

namespace GitHub.WindowsAzure.Table.Extensions
{
    public static class AsyncQueryableExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source) where T : new()
        {
            var tableQueryProvider = source.Provider as TableQueryProvider<T>;

            if (tableQueryProvider == null)
            {
                return source.ToList();
            }

            return ((IEnumerable<T>)await tableQueryProvider.ExecuteAsync(source.Expression)).ToList();
        }
    }
}