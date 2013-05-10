using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Infrastructure
{
    /// <summary>
    ///     Performs a constant value serialization.
    /// </summary>
    internal static class SerializationExtensions
    {
        /// <summary>
        ///     Collection of supported constant types.
        ///     http://www.odata.org/documentation/overview/#6_Primitive_Data_Types
        /// </summary>
        private static readonly Dictionary<Type, Func<object, String>> Serialization = new Dictionary<Type, Func<object, String>>
            {
                {typeof (String), o => string.Format("'{0}'", o)},
                {typeof (Boolean), o => o.ToString().ToLowerInvariant()},
                {typeof (Int32), o => string.Format(CultureInfo.InvariantCulture, "{0}", o)},
                {typeof (Int64), o => string.Format(CultureInfo.InvariantCulture, "{0}L", o)},
                {typeof (Single), o => string.Format(CultureInfo.InvariantCulture, "{0:#.0#}", o)},
                {typeof (Double), o => string.Format(CultureInfo.InvariantCulture, "{0:#.0#}", o)},
                {typeof (Guid), o => string.Format(CultureInfo.InvariantCulture, "guid'{0}'", o)},
                {
                    typeof (DateTime), o => string.Format(
                        "datetime'{0}'",
                        XmlConvert.ToString((DateTime) o, XmlDateTimeSerializationMode.RoundtripKind))
                },
                {
                    typeof (DateTimeOffset), o => string.Format(
                        "datetime'{0}'",
                        XmlConvert.ToString(((DateTimeOffset) o).DateTime, XmlDateTimeSerializationMode.RoundtripKind))
                },
                {
                    typeof (Byte[]), o =>
                        {
                            var stringBuilder = new StringBuilder("X'");

                            foreach (byte num in (Byte[]) o)
                            {
                                stringBuilder.AppendFormat("{0:x2}", num);
                            }

                            stringBuilder.Append("'");

                            return stringBuilder.ToString();
                        }
                }
            };

        /// <summary>
        ///     Collection of supported logical operands.
        ///     http://www.odata.org/documentation/uri-conventions/#45_Filter_System_Query_Option_filter
        /// </summary>
        private static readonly Dictionary<ExpressionType, String> LogicalOperators = new Dictionary<ExpressionType, String>
            {
                {ExpressionType.AndAlso, "and"},
                {ExpressionType.OrElse, "or"},
                {ExpressionType.Not, "not"},
                {ExpressionType.Equal, QueryComparisons.Equal},
                {ExpressionType.NotEqual, QueryComparisons.NotEqual},
                {ExpressionType.GreaterThan, QueryComparisons.GreaterThan},
                {ExpressionType.GreaterThanOrEqual, QueryComparisons.GreaterThanOrEqual},
                {ExpressionType.LessThan, QueryComparisons.LessThan},
                {ExpressionType.LessThanOrEqual, QueryComparisons.LessThanOrEqual}
            };

        /// <summary>
        ///     Serializes constant value.
        /// </summary>
        /// <param name="constant">Constant expression.</param>
        /// <returns>Serialized value.</returns>
        public static string Serialize(this ConstantExpression constant)
        {
            var value = constant.Value;
            if (value == null)
            {
                return "null";
            }

            // Trying to serialize constant value
            Func<object, string> serializer;
            if (!Serialization.TryGetValue(constant.Type, out serializer))
            {
                string message = String.Format(Resources.SerializationExtensionsNotSupportedType, constant.Type);
                throw new NotSupportedException(message);
            }

            return serializer(value);
        }

        /// <summary>
        ///     Serializes logical operator.
        /// </summary>
        /// <param name="type">Expression type.</param>
        /// <returns>Serialized value.</returns>
        public static string Serialize(this ExpressionType type)
        {
            string value;
            if (!LogicalOperators.TryGetValue(type, out value))
            {
                string message = String.Format(Resources.TranslatorOperatorNotSupported, type);
                throw new NotSupportedException(message);
            }

            return value;
        }

        /// <summary>
        ///     Determines whether operator is supported.
        /// </summary>
        /// <param name="type">Expression type.</param>
        /// <returns>True if supported otherwise false.</returns>
        public static bool IsSupported(this ExpressionType type)
        {
            return LogicalOperators.ContainsKey(type);
        }
    }
}