using System;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters;

namespace WindowsAzure.Table.QueryExecutor
{
    /// <summary>
    ///     Manages construction of table query executors.
    /// </summary>
    internal sealed class TableQueryExecutorFactory<T> where T : new()
    {
        internal readonly CloudTable CloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        public TableQueryExecutorFactory(CloudTable cloudTable, ITableEntityConverter<T> entityConverter)
        {
            if (cloudTable == null)
            {
                throw new ArgumentNullException("cloudTable");
            }

            if (entityConverter == null)
            {
                throw new ArgumentNullException("entityConverter");
            }

            CloudTable = cloudTable;
            _entityConverter = entityConverter;
        }

        /// <summary>
        ///     Creates a new <see cref="ITableQueryExecutor{TEntity}" />.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="executionMode">Execution mode.</param>
        /// <returns>
        ///     <see cref="ITableQueryExecutor{TEntity}" />
        /// </returns>
        public ITableQueryExecutor<T> Create(ExecutionMode executionMode)
        {
            if (executionMode == ExecutionMode.Parallel)
            {
                return new TableQueryParallelExecutor<T>(CloudTable, _entityConverter);
            }

            if (executionMode == ExecutionMode.Sequential)
            {
                return new TableQuerySequentialExecutor<T>(CloudTable, _entityConverter);
            }

            string message = string.Format(Resources.TableQueryExecutorInvalidMode, executionMode);
            throw new ArgumentOutOfRangeException("executionMode", message);
        }
    }
}