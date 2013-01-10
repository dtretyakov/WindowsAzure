using System.Collections.Generic;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    public interface IEntityTypeData<T>
    {
        /// <summary>
        ///     Gets a partiton key accessor.
        /// </summary>
        IValueAccessor<T> PartitionKey { get; }

        /// <summary>
        ///     Gets a row key accessor.
        /// </summary>
        IValueAccessor<T> RowKey { get; }

        /// <summary>
        ///     Gets a timestamp accessor.
        /// </summary>
        IValueAccessor<T> Timestamp { get; }

        /// <summary>
        ///     Gets a etag accessor.
        /// </summary>
        IValueAccessor<T> ETag { get; }

        /// <summary>
        ///     Gets a properties accessors.
        /// </summary>
        IEnumerable<IValueAccessor<T>> Properties { get; }

        /// <summary>
        ///     Gets an entity partition & row key name changes.
        /// </summary>
        IDictionary<string, string> NameChanges { get; }
    }
}