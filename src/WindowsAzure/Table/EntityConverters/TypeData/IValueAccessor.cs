using System;

namespace WindowsAzure.Table.EntityConverters.TypeData
{
    /// <summary>
    /// Value accessor.
    /// </summary>
    public interface IValueAccessor<in T>
    {
        Func<T, object> GetValue { get; }

        Action<T, object> SetValue { get; }

        Type Type { get; }

        string Name { get; }
    }
}