using System;
using System.Collections.Concurrent;
using WindowsAzure.Table.EntityConverters.Infrastructure;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Base class for a TableEntityConverter.
    ///     Keeps a retrieved EntityTypeData collection.
    /// </summary>
    public abstract class TableEntityConverterBase
    {
        protected static readonly ConcurrentDictionary<Type, EntityTypeData> TypesData =
            new ConcurrentDictionary<Type, EntityTypeData>();
    }
}