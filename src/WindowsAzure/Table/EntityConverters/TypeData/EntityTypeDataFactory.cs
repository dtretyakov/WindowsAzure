using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Manages entity type data collections.
    /// </summary>
    internal static class EntityTypeDataFactory
    {
        /// <summary>
        ///     Entity type map definition.
        /// </summary>
        private static readonly Type EntityTypeMap = typeof(EntityTypeMap<>);

        /// <summary>
        ///     Entity type data cache.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object> TypesData =
            new ConcurrentDictionary<Type, object>();

        /// <summary>
        ///     Retrieves an entity type data.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <returns>Entity type data.</returns>
        public static IEntityTypeData<T> GetEntityTypeData<T>() where T : class, new()
        {
            var type = typeof(T);
            var assembly = type.Assembly;

            return (IEntityTypeData<T>)TypesData.GetOrAdd(type,
                                                          key => FindsEntityTypeMap(assembly, type) ?? new EntityTypeData<T>());
        }

        /// <summary>
        ///     Registers an entity type data.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="entityTypeData">Entity type data.</param>
        public static void RegisterEntityTypeData<T>(IEntityTypeData<T> entityTypeData) where T : class, new()
        {
            TypesData.GetOrAdd(typeof(T), type => entityTypeData);
        }

        /// <summary>
        ///     Registers an entity type data.
        /// </summary>
        /// <param name="entityType">Entity type.</param>
        /// <param name="entityTypeData">Entity type data.</param>
        public static void RegisterEntityTypeData(Type entityType, object entityTypeData)
        {
            TypesData.GetOrAdd(entityType, type => entityTypeData);
        }

        /// <summary>
        ///     Finds an entity type mapping.
        /// </summary>
        /// <param name="assemblyToSearch">Aseembly to search mapping types.</param>
        /// <param name="entityType">Entity type.</typeparam>
        /// <returns>An instance of entity type mapping.</returns>
        private static object FindsEntityTypeMap(Assembly assemblyToSearch, Type entityType)
        {
            var types = assemblyToSearch.GetExportedTypes()
                                       .Where(IsMappingType)
                                       .ToList();

            foreach (var type in types)
            {
                Type entityTypeMatched;
                if (IsMappingType(type, out entityTypeMatched, t => EntityTypeMap.IsAssignableFrom(t))
                    && entityType == entityTypeMatched)
                {
                    try
                    {
                        var entityTypeData = Activator.CreateInstance(type);
                        RegisterEntityTypeData(entityType, entityTypeData);
                        return entityTypeData;
                    }
                    catch (TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
                }
                else
                {
                    continue;
                }
            }

            return null;
        }

        private static bool IsMappingType(Type mappingType)
        {
            if (!mappingType.IsClass || mappingType.IsAbstract)
            {
                return false;
            }

            Type temp;
            return IsMappingType(mappingType, out temp, t => EntityTypeMap.IsAssignableFrom(t));
        }

        private static bool IsMappingType(Type mappingType, out Type entityType, Predicate<Type> matcher)
        {
            entityType = null;

            while (mappingType != null)
            {
                if (mappingType.IsGenericType)
                {
                    var definationType = mappingType.GetGenericTypeDefinition();
                    if (matcher(definationType))
                    {
                        entityType = mappingType.GetGenericArguments().First();
                        return true;
                    }
                }

                mappingType = mappingType.BaseType;
            }

            return false;
        }
    }
}