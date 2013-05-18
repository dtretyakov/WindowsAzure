using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Queryable.Base;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Table.Wrappers;

namespace WindowsAzure.Table.Queryable
{
    /// <summary>
    ///     LINQ to Windows Azure Storage Table query provider.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx
    /// </summary>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    internal class TableQueryProvider<TEntity> : QueryProviderBase, IAsyncQueryProvider where TEntity : new()
    {
        private readonly ICloudTable _cloudTable;
        private readonly ITableEntityConverter<TEntity> _entityConverter;
        private readonly QueryTranslator _queryTranslator;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter"></param>
        internal TableQueryProvider(ICloudTable cloudTable, ITableEntityConverter<TEntity> entityConverter)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException("cloudTable");
            }

            if (entityConverter == null)
            {
                throw new ArgumentNullException("entityConverter");
            }

            _cloudTable = cloudTable;
            _entityConverter = entityConverter;
            _queryTranslator = new QueryTranslator(entityConverter.NameChanges);
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

            var result = new TranslationResult();

            _queryTranslator.Translate(expression, result);

            IEnumerable<DynamicTableEntity> tableEntities = _cloudTable.ExecuteQuery(result.TableQuery);

            return GetProcessedResult(tableEntities, result);
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

            var result = new TranslationResult();

            _queryTranslator.Translate(expression, result);

            return _cloudTable
                .ExecuteQueryAsync(result.TableQuery, cancellationToken)
                .Then(p => GetProcessedResult(p, result), cancellationToken);
        }

        /// <summary>
        ///     Executes post processing of retrieved entities.
        /// </summary>
        /// <param name="tableEntities">Table entities.</param>
        /// <param name="translation">translation result.</param>
        /// <returns>Collection of entities.</returns>
        private object GetProcessedResult(IEnumerable<DynamicTableEntity> tableEntities, TranslationResult translation)
        {
            IEnumerable<TEntity> result = tableEntities.Select(q => _entityConverter.GetEntity(q));

            if (translation.PostProcessing == null)
            {
                return result;
            }

            try
            {
                return translation.PostProcessing.DynamicInvoke(result.AsQueryable());
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }
    }
}