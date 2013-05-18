using System;
using System.ComponentModel;
using WindowsAzure.Properties;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Wrappers;

namespace WindowsAzure.Table.RequestExecutor
{
    /// <summary>
    ///     Handles construction of table request executors.
    /// </summary>
    internal sealed class TableRequestExecutorFactory<T> where T : new()
    {
        internal readonly ICloudTable CloudTable;
        private readonly ITableEntityConverter<T> _entityConverter;
        private readonly TableBatchPartitioner _partitioner;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="cloudTable">Cloud table.</param>
        /// <param name="entityConverter">Entity converter.</param>
        public TableRequestExecutorFactory(ICloudTable cloudTable, ITableEntityConverter<T> entityConverter)
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
            _partitioner = new TableBatchPartitioner();
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
                return new TableRequestParallelExecutor<T>(CloudTable, _entityConverter, _partitioner);
            }

            if (executionMode == ExecutionMode.Sequential)
            {
                return new TableRequestSequentialExecutor<T>(CloudTable, _entityConverter, _partitioner);
            }

            string message = string.Format(Resources.TableRequestExecutorInvalidMode, executionMode);
            throw new InvalidEnumArgumentException(message, (int) executionMode, typeof (ExecutionMode));
        }
    }
}