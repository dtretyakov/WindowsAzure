using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.EntityConverters.Infrastructure
{
    /// <summary>
    ///     Extensions for a TableEntityConverter.
    /// </summary>
    public static class ConverterExtensions
    {
        private static readonly Type DateTimeType = typeof (DateTime);

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
        private static readonly Dictionary<EdmType, Action<PropertyInfo, EntityProperty, Object>> EdmToType =
            new Dictionary<EdmType, Action<PropertyInfo, EntityProperty, Object>>
                {
                    {EdmType.Binary, (p, e, t) => p.SetValue(t, e.BinaryValue, null)},
                    {EdmType.Boolean, (p, e, t) => p.SetValue(t, e.BooleanValue, null)},
                    {EdmType.Double, (p, e, t) => p.SetValue(t, e.DoubleValue, null)},
                    {EdmType.Guid, (p, e, t) => p.SetValue(t, e.GuidValue, null)},
                    {EdmType.Int32, (p, e, t) => p.SetValue(t, e.Int32Value, null)},
                    {EdmType.Int64, (p, e, t) => p.SetValue(t, e.Int64Value, null)},
                    {EdmType.String, (p, e, t) => p.SetValue(t, e.StringValue, null)},
                    {
                        EdmType.DateTime,
                        (p, e, t) =>
                            {
                                if (p.PropertyType == DateTimeType)
                                {
                                    if (e.DateTimeOffsetValue != null)
                                    {
                                        p.SetValue(t, e.DateTimeOffsetValue.Value.DateTime, null);
                                    }
                                }
                                else
                                {
                                    p.SetValue(t, e.DateTimeOffsetValue, null);
                                }
                            }
                    },
                };

        /// <summary>
        ///     Returns a string property value.
        /// </summary>
        /// <param name="property">Property info.</param>
        /// <param name="target">Target objecy.</param>
        /// <returns>Property value.</returns>
        public static String GetStringValue(this PropertyInfo property, Object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            return (String) property.GetValue(target, null);
        }

        /// <summary>
        ///     Gets an entity property value by property info.
        /// </summary>
        /// <param name="property">Property info.</param>
        /// <param name="target">Target object.</param>
        /// <returns>Entity property.</returns>
        public static EntityProperty GetEntityProperty(this PropertyInfo property, Object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            Object value = property.GetValue(target, null);

            if (!TypeToEdm.ContainsKey(property.PropertyType))
            {
                throw new Exception(String.Format("Invalid property type: {0}", property.Name));
            }

            return TypeToEdm[property.PropertyType](value);
        }

        /// <summary>
        ///     Sets an object property value from entity property.
        /// </summary>
        /// <param name="property">Property info.</param>
        /// <param name="entityProperty">Entity property.</param>
        /// <param name="target">Target object.</param>
        public static void SetPropertyValue(this PropertyInfo property, EntityProperty entityProperty, Object target)
        {
            if (entityProperty == null)
            {
                throw new ArgumentNullException("entityProperty");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (!EdmToType.ContainsKey(entityProperty.PropertyType))
            {
                throw new Exception("Invalid entity property EDM type.");
            }

            EdmToType[entityProperty.PropertyType](property, entityProperty, target);
        }
    }
}