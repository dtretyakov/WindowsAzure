using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using GitHub.WindowsAzure.Table.EntityConverters;
using GitHub.WindowsAzure.Table.Extensions;
using GitHub.WindowsAzure.Table.Queryable.Base;
using GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators;
using GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators.Methods;
using Microsoft.WindowsAzure.Storage.Table;

namespace GitHub.WindowsAzure.Table.Queryable
{
    /// <summary>
    ///     Windows Azure Table Linq query provider.
    ///     <see cref="http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx" />
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TableQueryProvider<TEntity> : QueryProviderBase, IAsyncQueryProvider<TEntity> where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityConverter<TEntity> _converter;
        private readonly IDictionary<string, IMethodTranslator> _methodTranslators;

        /// <summary>
        ///     Contructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="converter">Entities converter.</param>
        public TableQueryProvider(CloudTableClient cloudTableClient, string tableName,
                                  ITableEntityConverter<TEntity> converter)
        {
            _converter = converter;
            _cloudTable = cloudTableClient.GetTableReference(tableName);

            _methodTranslators = new Dictionary<string, IMethodTranslator>
                                     {
                                         {QueryConstants.Where, new WhereTranslator()},
                                         {QueryConstants.Select, new SelectTranslator()},
                                         {QueryConstants.Take, new TakeTranslator()}
                                     };
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="converter">Entities converter.</param>
        /// <param name="methodTranslators">List of expression method translators.</param>
        public TableQueryProvider(CloudTableClient cloudTableClient, string tableName,
                                  ITableEntityConverter<TEntity> converter,
                                  IDictionary<string, IMethodTranslator> methodTranslators)
        {
            _converter = converter;
            _methodTranslators = methodTranslators;
            _cloudTable = cloudTableClient.GetTableReference(tableName);
        }

        /// <summary>
        ///     Executes expression query.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>Result.</returns>
        public override object Execute(Expression expression)
        {
            TableQuery query = GetTableQuery(expression);

            var entities = _cloudTable.ExecuteQuery(query);

            // NOTE: Waiting for fixing
            // https://github.com/WindowsAzure/azure-sdk-for-net/issues/144
            if (query.TakeCount.HasValue)
            {
                entities = entities.Take(query.TakeCount.Value);
            }

            return entities.Select(p => _converter.GetEntity(p));
        }

        /// <summary>
        ///     Creates a table query by expression.
        /// </summary>
        /// <param name="expression">Query expression.</param>
        /// <returns>Table query.</returns>
        public virtual TableQuery GetTableQuery(Expression expression)
        {
            var query = new TableQuery();
            var translator = new QueryTranslator(_methodTranslators);

            IDictionary<string, string> queryResult = translator.Translate(expression);

            // Select method
            if (queryResult.ContainsKey(QueryConstants.Select))
            {
                query.Select(queryResult[QueryConstants.Select].Split(','));
            }

            // Where method
            if (queryResult.ContainsKey(QueryConstants.Where))
            {
                query.FilterString = queryResult[QueryConstants.Where];
            }

            // Contains method
            if (queryResult.ContainsKey(QueryConstants.Take))
            {
                int takeCount;

                if (int.TryParse(queryResult[QueryConstants.Take], out takeCount))
                {
                    query.TakeCount = takeCount;
                }
            }

            return query;
        }

        public async Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            TableQuery query = GetTableQuery(expression);

            var entities = await _cloudTable.ExecuteQueryAsync(query);

            return entities.Select(p => _converter.GetEntity(p));
        }

        public async Task<TEntity> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            TableQuery query = GetTableQuery(expression);

            query.TakeCount = 1;

            var entities = await _cloudTable.ExecuteQueryAsync(query);

            return entities.Select(p => _converter.GetEntity(p)).FirstOrDefault();
        }
    }
}