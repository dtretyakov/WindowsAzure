using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.RequestExecutor;
using WindowsAzure.Table.Wrappers;

namespace WindowsAzure.Tests.Common
{
    public static class MocksFactory
    {
        internal static Mock<ITableRequestExecutor<TEntity>> GetQueryExecutorMock<TEntity>() where TEntity : new()
        {
            var mock = new Mock<ITableRequestExecutor<TEntity>>();

            // Execute
            mock.Setup(executor => executor.Execute(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation) => entity);

            mock.Setup(executor => executor.ExecuteWithoutResult(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>()));

            // Execute async
            mock.Setup(executor => executor.ExecuteAsync(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => Task.FromResult(entity));

            mock.Setup(executor => executor.ExecuteWithoutResultAsync(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => TaskHelpers.Completed());

            // Execute batches
            mock.Setup(executor => executor.ExecuteBatches(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((IEnumerable<TEntity> entities, Func<ITableEntity, TableOperation> operation) => entities);

            mock.Setup(executor => executor.ExecuteBatchesWithoutResult(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()));

            // Execute batches async
            mock.Setup(executor => executor.ExecuteBatchesAsync(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<TEntity> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => Task.FromResult(entities));

            mock.Setup(executor => executor.ExecuteBatchesWithoutResultAsync(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<TEntity> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => TaskHelpers.Completed());

            return mock;
        }

        internal static Mock<ITableEntityConverter<T>> GetTableEntityConverterMock<T>() where T : new()
        {
            var mock = new Mock<ITableEntityConverter<T>>();

            mock.Setup(converter => converter.GetEntity(It.IsAny<DynamicTableEntity>())).Returns(() => default(T));
            mock.Setup(converter => converter.GetEntity(It.IsAny<T>())).Returns(() => new DynamicTableEntity("pk", "rk"));

            return mock;
        }

        internal static Mock<ICloudTable> GetCloudTableMock()
        {
            var mock = new Mock<ICloudTable>();

            mock.Setup(p => p.Execute(It.IsAny<TableOperation>())).Returns(() => new TableResult());
            mock.Setup(p => p.ExecuteAsync(It.IsAny<TableOperation>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult(new TableResult()));
            mock.Setup(p => p.ExecuteBatch(It.IsAny<TableBatchOperation>()))
                .Returns(() => new List<TableResult>
                    {
                        new TableResult {Result = new DynamicTableEntity("pk", "rk")}
                    });
            mock.Setup(p => p.ExecuteBatchAsync(It.IsAny<TableBatchOperation>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult((IList<TableResult>) new List<TableResult>
                    {
                        new TableResult {Result = new DynamicTableEntity("pk", "rk")}
                    }));
            mock.Setup(p => p.ExecuteQuery(It.IsAny<ITableQuery>())).Returns(() => new List<DynamicTableEntity>());
            mock.Setup(p => p.ExecuteQueryAsync(It.IsAny<ITableQuery>(), It.IsAny<CancellationToken>()))
                .Returns(() => Task.FromResult((IEnumerable<DynamicTableEntity>) new List<DynamicTableEntity>()));

            return mock;
        }

        internal static Mock<ITableBatchPartitioner> GetTableBatchPartitionerMock()
        {
            var mock = new Mock<ITableBatchPartitioner>();
            const int batchCount = 100;

            mock.Setup(p => p.GetBatches(It.IsAny<IEnumerable<ITableEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((IEnumerable<ITableEntity> tableEntities, Func<ITableEntity, TableOperation> operation) =>
                    {
                        var batches = new Dictionary<string, TableBatchOperation>();
                        var result = new List<TableBatchOperation>();

                        foreach (ITableEntity tableEntity in tableEntities)
                        {
                            TableBatchOperation batch;

                            if (!batches.TryGetValue(tableEntity.PartitionKey, out batch))
                            {
                                batch = new TableBatchOperation();
                                batches.Add(tableEntity.PartitionKey, batch);
                            }

                            batch.Add(operation(tableEntity));

                            if (batch.Count == batchCount)
                            {
                                batches.Remove(tableEntity.PartitionKey);
                                result.Add(batch);
                            }
                        }

                        result.AddRange(batches.Select(pair => pair.Value));

                        return result;
                    });


            return mock;
        }
    }
}