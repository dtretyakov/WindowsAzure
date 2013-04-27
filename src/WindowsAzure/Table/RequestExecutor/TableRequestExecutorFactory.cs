using System;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters;

namespace WindowsAzure.Table.RequestExecutor
{
    /// <summary>
    ///     Handles construction of table request executors.
    /// </summary>
    internal sealed class TableRequestExecutorFactory<T> where T : new()
    {
        internal readonly CloudTable CloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        public TableRequestExecutorFactory(CloudTable cloudTable, ITableEntityConverter<T> entityConverter)
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
        ///     Creates a new <see cref="ITableRequestExecutor{T}" />.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="executionMode">Execution mode.</param>
        /// <returns>
        ///     <see cref="ITableRequestExecutor{T}" />
        /// </returns>
        public ITableRequestExecutor<T> Create(ExecutionMode executionMode)
        {
            if (executionMode == ExecutionMode.Parallel)
            {
                return new TableRequestParallelExecutor<T>(CloudTable, _entityConverter);
            }

            if (executionMode == ExecutionMode.Sequential)
            {
                return new TableRequestSequentialExecutor<T>(CloudTable, _entityConverter);
            }

            string message = string.Format(Resources.TableRequestExecutorInvalidMode, executionMode);
            throw new ArgumentOutOfRangeException("executionMode", message);
        }
    }
}