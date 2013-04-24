using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table;
using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.QueryExecutor;

namespace WindowsAzure.Tests.Common
{
    public static class MocksFactory
    {
        internal static Mock<ITableQueryExecutor<TEntity>> GetQueryExecutorMock<TEntity>() where TEntity : new()
        {
            var mock = new Mock<ITableQueryExecutor<TEntity>>();

            // Execute
            mock.Setup(executor => executor.Execute(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation) => entity);

            // Execute async
            mock.Setup(executor => executor.ExecuteAsync(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => Task.FromResult(entity));

            // Execute batches
            mock.Setup(executor => executor.ExecuteBatches(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((IEnumerable<TEntity> entities, Func<ITableEntity, TableOperation> operation) => entities);

            // Execute batches async
            mock.Setup(executor => executor.ExecuteBatchesAsync(It.IsAny<IEnumerable<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((IEnumerable<TEntity> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => Task.FromResult(entities));

            return mock;
        }

        internal static Mock<ITableEntityConverter<T>> GetTableEntityConverterMock<T>() where T : new()
        {
            var mock = new Mock<ITableEntityConverter<T>>();

            mock.Setup(converter => converter.GetEntity(It.IsAny<DynamicTableEntity>())).Returns(() => default(T));

            mock.Setup(converter => converter.GetEntity(It.IsAny<T>())).Returns(() => new DynamicTableEntity());

            return mock;
        }
    }
}