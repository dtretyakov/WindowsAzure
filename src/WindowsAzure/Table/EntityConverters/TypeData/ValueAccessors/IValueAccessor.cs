using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Value accessor.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IValueAccessor<in T>
    {
        /// <summary>
        ///     Gets an entity property value.
        /// </summary>
        Func<T, EntityProperty> GetValue { get; }

        /// <summary>
        ///     Sets an entity property value.
        /// </summary>
        Action<T, EntityProperty> SetValue { get; }

        /// <summary>
        ///     Member type.
        /// </summary>
        Type Type { get; }

        /// <summary>
        ///     Member name.
        /// </summary>
        string Name { get; }
    }
}