using System.Reflection;
using WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors;

namespace WindowsAzure.Table.EntityConverters.TypeData.Properties
{
    /// <summary>
    ///     Handles access to the key property.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    internal interface IKeyProperty<T> : IProperty<T>
    {
        /// <summary>
        ///     Gets a value indication whether property has accessor.
        /// </summary>
        bool HasAccessor { get; }

        /// <summary>
        ///     Checks whether memberInfo contains an specific criteria.
        /// </summary>
        /// <param name="memberInfo">Member info.</param>
        /// <param name="accessor">Value accessor.</param>
        /// <returns>Result.</returns>
        bool Validate(MemberInfo memberInfo, IValueAccessor<T> accessor);
    }
}