using System;
using System.Collections.Concurrent;
using WindowsAzure.Table.EntityConverters.TypeData;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Manages entity type data collections.
    /// </summary>
    public static class EntityTypeDataFactory
    {
        private static readonly ConcurrentDictionary<Type, object> TypesData =
            new ConcurrentDictionary<Type, object>();

        /// <summary>
        ///     Retreives an entity type data.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>Entity type data.</returns>
        public static IEntityTypeData<T> GetEntityTypeData<T>()
        {
            return (IEntityTypeData<T>) TypesData.GetOrAdd(typeof (T), type => new EntityTypeData<T>());
        }
    }
}