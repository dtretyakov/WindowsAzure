using System;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.TypeData.ValueAccessors
{
    /// <summary>
    ///     Creates entity properties.
    /// </summary>
    public static class EntityPropertyFactory
    {
        /// <summary>
        ///     Creates Boolean entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Boolean value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Byte entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Byte[] value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates DateTimeOffset entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(DateTimeOffset value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates DateTime entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(DateTime value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Double entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Double value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Guid entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Guid value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Int32 entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Int32 value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Int64 entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Int64 value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates String entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(String value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Boolean? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Boolean? value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates DateTimeOffset? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(DateTimeOffset? value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates DateTime? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(DateTime? value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Double? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Double? value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Guid? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Guid? value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Int32? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Int32? value)
        {
            return new EntityProperty(value);
        }

        /// <summary>
        ///     Creates Int64? entity property.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty Create(Int64? value)
        {
            return new EntityProperty(value);
        }
    }
}