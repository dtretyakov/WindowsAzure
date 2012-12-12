using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using GitHub.WindowsAzure.Table.EntityFormatters;
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
    public class TableQueryProvider<TEntity> : QueryProviderBase where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityFormatter<TEntity> _formatter;
        private readonly IDictionary<string, IMethodTranslator> _methodTranslators;

        /// <summary>
        ///     Contructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="formatter">Entities formatter.</param>
        public TableQueryProvider(CloudTableClient cloudTableClient, string tableName,
                                  ITableEntityFormatter<TEntity> formatter)
        {
            _formatter = formatter;
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
        /// <param name="formatter">Entities formatter.</param>
        /// <param name="methodTranslators">List of expression methods formatters.</param>
        public TableQueryProvider(CloudTableClient cloudTableClient, string tableName,
                                  ITableEntityFormatter<TEntity> formatter,
                                  IDictionary<string, IMethodTranslator> methodTranslators)
        {
            _formatter = formatter;
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

            List<DynamicTableEntity> entities = _cloudTable.ExecuteQuery(query).ToList();

            IEnumerable<TEntity> result = entities.Select(p => _formatter.GetEntity(p));

            return result;
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
    }
}