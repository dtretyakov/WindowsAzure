using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Table.Queryable.Base;
using WindowsAzure.Table.Queryable.Expressions;

namespace WindowsAzure.Table.Queryable
{
    /// <summary>
    ///     Windows Azure Table Linq query provider.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    public class TableQueryProvider<TEntity> : QueryProviderBase, IAsyncQueryProvider where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly TableSetConfiguration<TEntity> _configuration;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="configuration">LINQ Expression translator.</param>
        public TableQueryProvider(CloudTable cloudTable, TableSetConfiguration<TEntity> configuration)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException("cloudTable");
            }

            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            _cloudTable = cloudTable;
            _configuration = configuration;
        }

        /// <summary>
        ///     Executes expression query.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>Result.</returns>
        public override object Execute(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            TableQuery query = GetTableQuery(expression);

            IEnumerable<DynamicTableEntity> entities = _cloudTable.ExecuteQuery(query);

            // NOTE: Waiting for fixing
            // https://github.com/WindowsAzure/azure-sdk-for-net/issues/144
            if (query.TakeCount.HasValue)
            {
                entities = entities.Take(query.TakeCount.Value);
            }

            return entities.Select(p => _configuration.EntityConverter.GetEntity(p));
        }

        /// <summary>
        ///     Executes expression query asynchronously.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<object> ExecuteAsync(
            Expression expression,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            TableQuery query = GetTableQuery(expression);

            return _cloudTable
                .ExecuteQueryAsync(query)
                .Then(entities => (object) entities.Select(p => _configuration.EntityConverter.GetEntity(p)), cancellationToken);
        }

        /// <summary>
        ///     Creates a table query by expression.
        /// </summary>
        /// <param name="expression">Query expression.</param>
        /// <returns>Table query.</returns>
        public TableQuery GetTableQuery(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            var queryTranslator = new QueryTranslator(_configuration.EntityConverter.NameChanges);

            IDictionary<QuerySegment, string> queryResult = queryTranslator.Translate(expression);

            var query = new TableQuery();

            // Select
            if (queryResult.ContainsKey(QuerySegment.Select))
            {
                query.Select(queryResult[QuerySegment.Select].Split(','));
            }

            // Filter
            if (queryResult.ContainsKey(QuerySegment.Filter))
            {
                query.FilterString = queryResult[QuerySegment.Filter];
            }

            // Top
            if (queryResult.ContainsKey(QuerySegment.Top))
            {
                int takeCount;

                if (int.TryParse(queryResult[QuerySegment.Top], out takeCount))
                {
                    query.TakeCount = takeCount;
                }
            }

            return query;
        }
    }
}