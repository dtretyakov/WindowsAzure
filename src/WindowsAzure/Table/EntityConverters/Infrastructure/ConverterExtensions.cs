using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.Infrastructure
{
    /// <summary>
    ///     Extensions for a TableEntityConverter.
    /// </summary>
    public static class ConverterExtensions
    {
        /// <summary>
        ///     Type to edm type convertion collection.
        /// </summary>
        private static readonly Dictionary<Type, Func<object, EntityProperty>> TypeToEdm =
            new Dictionary<Type, Func<object, EntityProperty>>
                {
                    {typeof (String), o => new EntityProperty((String) o)},
                    {typeof (Byte[]), o => new EntityProperty((Byte[]) o)},
                    {typeof (Boolean), o => new EntityProperty((Boolean) o)},
                    {typeof (DateTime), o => new EntityProperty((DateTime) o)},
                    {typeof (DateTimeOffset), o => new EntityProperty((DateTimeOffset) o)},
                    {typeof (Double), o => new EntityProperty((Double) o)},
                    {typeof (Guid), o => new EntityProperty((Guid) o)},
                    {typeof (Int32), o => new EntityProperty((Int32) o)},
                    {typeof (Int64), o => new EntityProperty((Int64) o)}
                };

        /// <summary>
        ///     Edm type to type convertion collection.
        /// </summary>
        private static readonly Dictionary<EdmType, Func<EntityProperty, object>> EdmToType =
            new Dictionary<EdmType, Func<EntityProperty, object>>
                {
                    {EdmType.Binary, p => p.BinaryValue},
                    {EdmType.Boolean, p => p.BooleanValue},
                    {EdmType.Double, p => p.DoubleValue},
                    {EdmType.Guid, p => p.GuidValue},
                    {EdmType.Int32, p => p.Int32Value},
                    {EdmType.Int64, p => p.Int64Value},
                    {EdmType.String, p => p.StringValue},
                    {EdmType.DateTime,p => p.DateTimeOffsetValue.Value.DateTime}
                };

        /// <summary>
        ///     Gets an entity property value by property info.
        /// </summary>
        /// <param name="propertyType">Property type.</param>
        /// <param name="value">Value.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty GetEntityProperty(this Type propertyType, object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            if (!TypeToEdm.ContainsKey(propertyType))
            {
                throw new ArgumentOutOfRangeException("propertyType");
            }

            return TypeToEdm[propertyType](value);
        }

        /// <summary>
        ///     Sets an object property value from entity property.
        /// </summary>
        /// <param name="entityProperty">Entity property.</param>
        public static object GetValue(this EntityProperty entityProperty)
        {
            if (entityProperty == null)
            {
                throw new ArgumentNullException("entityProperty");
            }

            if (!EdmToType.ContainsKey(entityProperty.PropertyType))
            {
                throw new Exception("Invalid entity property EDM type.");
            }

            return EdmToType[entityProperty.PropertyType](entityProperty);
        }
    }
}