using System;
using System.Collections.Concurrent;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Manages entity type data collections.
    /// </summary>
    internal static class EntityTypeDataFactory
    {
        /// <summary>
        ///     Entity type data cache.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object> TypesData =
            new ConcurrentDictionary<Type, object>();

        /// <summary>
        ///     Retreives an entity type data.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>Entity type data.</returns>
        public static IEntityTypeData<T> GetEntityTypeData<T>() where T : class, new()
        {
            return (IEntityTypeData<T>) TypesData.GetOrAdd(typeof (T), type => new EntityTypeData<T>());
        }
    }
}