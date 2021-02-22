﻿using System;
using System.Collections.Generic;
#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#else
using Microsoft.Azure.Cosmos.Table;
#endif

namespace WindowsAzure.Table.RequestExecutor
{
    /// <summary>
    ///     Handles entities batching in insert operations.
    /// </summary>
    internal interface ITableBatchPartitioner
    {
        /// <summary>
        ///     Generates a list of batches according to partitioning settings.
        /// </summary>
        /// <param name="tableEntities">Entities collection.</param>
        /// <param name="operation">Table operation.</param>
        /// <returns>Batches.</returns>
        IEnumerable<TableBatchOperation> GetBatches(IEnumerable<ITableEntity> tableEntities, Func<ITableEntity, TableOperation> operation);
    }
}