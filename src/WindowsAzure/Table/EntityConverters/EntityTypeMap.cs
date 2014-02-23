using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Properties;
using WindowsAzure.Table.Attributes;
using WindowsAzure.Table.EntityConverters.TypeData;
using WindowsAzure.Table.EntityConverters.TypeData.Properties;

namespace WindowsAzure.Table.EntityConverters
{
    /// <summary>
    ///     Maps an entity type data.
    /// </summary>
    public static class EntityTypeMap
    {
        /// <summary>
        ///     Creates and registers a class map.
        /// </summary>
        /// <typeparam name="TClass">Entity type.</typeparam>
        /// <param name="classMapInitializer">The class map initializer.</param>
        /// <returns>The class map.</returns>
        public static EntityTypeMappedData<TClass> RegisterClassMap<TClass>(Action<EntityTypeMappedData<TClass>> classMapInitializer)
            where TClass : class, new()
        {
            var entityTypeMap = new EntityTypeMappedData<TClass>(classMapInitializer);
            EntityTypeDataFactory.RegisterEntityTypeData(entityTypeMap);
            return entityTypeMap;
        }
    }
}