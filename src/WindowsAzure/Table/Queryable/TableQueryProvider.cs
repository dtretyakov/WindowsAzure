using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Extensions;
using WindowsAzure.Table.Queryable.Base;
using WindowsAzure.Table.Queryable.Expressions;

namespace WindowsAzure.Table.Queryable
{
    /// <summary>
    ///     Windows Azure Table Linq query provider.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class TableQueryProvider<TEntity> : QueryProviderBase, IAsyncQueryProvider where TEntity : new()
    {
        private readonly CloudTable _cloudTable;
        private readonly ITableEntityConverter<TEntity> _converter;
        private readonly IQueryTranslator _queryTranslator;

        /// <summary>
        ///     Contructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="converter">Entities converter.</param>
        public TableQueryProvider(CloudTableClient cloudTableClient, string tableName,
                                  ITableEntityConverter<TEntity> converter)
            : this(cloudTableClient, tableName, converter, new QueryTranslator(converter.NameChanges))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTableClient">Cloud table client.</param>
        /// <param name="tableName">Table name.</param>
        /// <param name="converter">Entities converter.</param>
        /// <param name="queryTranslator">LINQ Expression translator.</param>
        public TableQueryProvider(CloudTableClient cloudTableClient, string tableName,
                                  ITableEntityConverter<TEntity> converter,
                                  IQueryTranslator queryTranslator)
        {
            if (cloudTableClient == null)
            {
                throw new ArgumentNullException("cloudTableClient");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            if (converter == null)
            {
                throw new ArgumentNullException("converter");
            }

            if (queryTranslator == null)
            {
                throw new ArgumentNullException("queryTranslator");
            }

            _converter = converter;
            _queryTranslator = queryTranslator;
            _cloudTable = cloudTableClient.GetTableReference(tableName);
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

            return entities.Select(p => _converter.GetEntity(p));
        }

        /// <summary>
        ///     Executes
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<object> ExecuteAsync(Expression expression,
                                               CancellationToken cancellationToken = default(CancellationToken))
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            TableQuery query = GetTableQuery(expression);

            return _cloudTable
                .ExecuteQueryAsync(query)
                .Then(entities => (object) entities.Select(p => _converter.GetEntity(p)), cancellationToken);
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

            IDictionary<QuerySegment, string> queryResult = _queryTranslator.Translate(expression);

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