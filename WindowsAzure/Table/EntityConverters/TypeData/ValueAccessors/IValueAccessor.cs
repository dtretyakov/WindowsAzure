using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Entity member value accessor.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IValueAccessor<in T>
    {
        /// <summary>
        ///     Gets an entity memeber accessor.
        /// </summary>
        Func<T, EntityProperty> GetValue { get; }

        /// <summary>
        ///     Sets an entity member mutator.
        /// </summary>
        Action<T, EntityProperty> SetValue { get; }

        /// <summary>
        ///     Gets a member type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Gets a member name.
        /// </summary>
        string Name { get; }
    }
}