using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using Moq;
using WindowsAzure.Table;

namespace WindowsAzure.Tests.Common
{
    public static class MocksFactory
    {
        public static Mock<ITableQueryExecutor<TEntity>> GetQueryExecutorMock<TEntity>() where TEntity : new()
        {
            var mock = new Mock<ITableQueryExecutor<TEntity>>();

            // Execute
            mock.Setup(executor => executor.Execute(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation) => entity);

            // Execute async
            mock.Setup(executor => executor.ExecuteAsync(It.IsAny<TEntity>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((TEntity entity, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => Task.FromResult(entity));

            // Execute batches
            mock.Setup(executor => executor.ExecuteBatches(It.IsAny<IList<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>()))
                .Returns((IList<TEntity> entities, Func<ITableEntity, TableOperation> operation) => entities);

            // Execute batches async
            mock.Setup(executor => executor.ExecuteBatchesAsync(It.IsAny<IList<TEntity>>(), It.IsAny<Func<ITableEntity, TableOperation>>(), It.IsAny<CancellationToken>()))
                .Returns((IList<TEntity> entities, Func<ITableEntity, TableOperation> operation, CancellationToken cancellationToken) => Task.FromResult(entities));

            return mock;
        }
    }
}