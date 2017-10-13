using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using WindowsAzure.Common;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    ///     Manages entity type data collections.
    /// </summary>
    internal static class EntityTypeDataFactory
    {
        /// <summary>
        ///     Assemblies to find entity type map.
        /// </summary>
        private static Assembly[] _mappingAssemblies = {};

        /// <summary>
        ///     Entity type map definition.
        /// </summary>
        private static readonly Type EntityTypeMap = typeof (EntityTypeMap<>);

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
            var type = typeof (T);
            var assemblies = _mappingAssemblies.Concat(new[] {type.GetTypeInfo().Assembly})
                .Distinct();

            return (IEntityTypeData<T>) TypesData.GetOrAdd(type,
               key => FindsEntityTypeMap(assemblies, type) ?? new EntityTypeData<T>());
        }

        /// <summary>
        ///     Registers an entity type data.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="entityTypeData">Entity type data.</param>
        public static void RegisterEntityTypeData<T>(IEntityTypeData<T> entityTypeData) where T : class, new()
        {
            TypesData.GetOrAdd(typeof (T), type => entityTypeData);
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
        ///     Registers an assembly with entity type mappings.
        /// </summary>
        /// <param name="assemblies"></param>
        public static void RegisterMappingAssembly(params Assembly[] assemblies)
        {
            _mappingAssemblies = assemblies;
        }

        /// <summary>
        ///     Finds an entity type mapping.
        /// </summary>
        /// <param name="assembliesToSearch">Aseemblies to search mapping types.</param>
        /// <param name="entityType">Entity type.</param>
        /// <returns>An instance of entity type mapping.</returns>
        private static object FindsEntityTypeMap(IEnumerable<Assembly> assembliesToSearch, Type entityType)
        {
            foreach (var assembly in assembliesToSearch)
            {
                var types = assembly.GetExportedTypes()
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

                            var e = entityTypeData as EntityTypeMap;
                            e?.AutoMap();

                            RegisterEntityTypeData(entityType, entityTypeData);

                            return entityTypeData;
                        }
                        catch (TargetInvocationException ex)
                        {
                            throw ex.InnerException ?? ex;
                        }
                    }
                }
            }

            return null;
        }

        private static bool IsMappingType(Type mappingType)
        {
            if (mappingType.IsAbstractType())
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
                var typeInfo = mappingType.GetTypeInfo();
                if (typeInfo.IsGenericType)
                {
                    var definationType = mappingType.GetGenericTypeDefinition();
                    if (matcher(definationType))
                    {
                        entityType = mappingType.GetGenericArguments().First();
                        return true;
                    }
                }

                mappingType = typeInfo.BaseType;
            }

            return false;
        }
    }
}