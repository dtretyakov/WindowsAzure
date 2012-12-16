using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Table.Queryable.Base;
using WindowsAzure.Table.Queryable.ExpressionTranslators;
using WindowsAzure.Table.Queryable.ExpressionTranslators.Methods;

namespace WindowsAzure.Table.Queryable
{
    /// <summary>
    ///     Windows Azure Table Linq query provider.
    ///     <see cref="http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx" />
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TableQueryProvider<TEntity> : QueryProviderBase, IAsyncQueryProvider where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityConverter<TEntity> _converter;
        private readonly IList<IMethodTranslator> _methodTranslators;

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

            _methodTranslators = new List<IMethodTranslator>
                                     {
                                         new SelectTranslator(),
                                         new TakeTranslator(),
                                         new WhereTranslator()
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
                                  IList<IMethodTranslator> methodTranslators)
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

            IEnumerable<DynamicTableEntity> entities = _cloudTable.ExecuteQuery(query);

            // NOTE: Waiting for fixing
            // https://github.com/WindowsAzure/azure-sdk-for-net/issues/144
            if (query.TakeCount.HasValue)
            {
                entities = entities.Take(query.TakeCount.Value);
            }

            return entities.Select(p => _converter.GetEntity(p));
        }

        /// <summary>
        ///     Executes
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<object> ExecuteAsync(Expression expression,
                                               CancellationToken cancellationToken = default(CancellationToken))
        {
            TableQuery query = GetTableQuery(expression);

            IEnumerable<DynamicTableEntity> entities = await _cloudTable.ExecuteQueryAsync(query);

            return entities.Select(p => _converter.GetEntity(p));
        }

        /// <summary>
        ///     Returns a query translator.
        /// </summary>
        /// <returns></returns>
        private IQueryTranslator GetQueryTranslator()
        {
            return new QueryTranslator(_converter.NameMappings, _methodTranslators);
        }

        /// <summary>
        ///     Creates a table query by expression.
        /// </summary>
        /// <param name="expression">Query expression.</param>
        /// <returns>Table query.</returns>
        public virtual TableQuery GetTableQuery(Expression expression)
        {
            IDictionary<QueryConstants, string> queryResult = GetQueryTranslator().Translate(expression);

            var query = new TableQuery();

            // Select method
            if (queryResult.ContainsKey(QueryConstants.Select))
            {
                query.Select(queryResult[QueryConstants.Select].Split(','));
            }

            // Where method
            if (queryResult.ContainsKey(QueryConstants.Filter))
            {
                query.FilterString = queryResult[QueryConstants.Filter];
            }

            // Contains method
            if (queryResult.ContainsKey(QueryConstants.Top))
            {
                int takeCount;

                if (int.TryParse(queryResult[QueryConstants.Top], out takeCount))
                {
                    query.TakeCount = takeCount;
                }
            }

            return query;
        }
    }
}