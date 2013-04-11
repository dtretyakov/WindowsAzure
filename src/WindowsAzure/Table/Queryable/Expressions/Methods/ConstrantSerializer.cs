using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Xml;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Performs a constant value serialization.
    /// </summary>
    public sealed class ConstrantSerializer
    {
        /// <summary>
        ///     Collection of supported constant types.
        /// </summary>
        private readonly Dictionary<Type, Func<object, String>> _serialization =
            new Dictionary<Type, Func<object, String>>
                {
                    {typeof (String), o => string.Format("'{0}'", o)},
                    {
                        typeof (DateTime), o => string.Format(
                            "datetime'{0}'",
                            XmlConvert.ToString((DateTime) o, XmlDateTimeSerializationMode.RoundtripKind))
                    },
                    {
                        typeof (DateTimeOffset), o => string.Format(
                            "datetime'{0}'",
                            XmlConvert.ToString(((DateTimeOffset) o).DateTime,
                                                XmlDateTimeSerializationMode.RoundtripKind))
                    },
                    {typeof (Boolean), o => o.ToString().ToLowerInvariant()},
                    {typeof (Int32), o => string.Format(CultureInfo.InvariantCulture, "{0}", o)},
                    {typeof (Int64), o => string.Format(CultureInfo.InvariantCulture, "{0}L", o)},
                    {typeof (Single), o => string.Format(CultureInfo.InvariantCulture, "{0:#.0#}", o)},
                    {typeof (Double), o => string.Format(CultureInfo.InvariantCulture, "{0:#.0#}", o)},
                    {typeof (Guid), o => string.Format(CultureInfo.InvariantCulture, "guid'{0}'", o)},
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
        ///     Serializes constrant value.
        /// </summary>
        /// <param name="constant">Constant expression.</param>
        /// <returns>Result.</returns>
        public string Serialize(ConstantExpression constant)
        {
            if (constant.Value == null)
            {
                return "null";
            }

            Type constantType = constant.Value.GetType();

            // Trying to serialize constant value
            if (!_serialization.ContainsKey(constantType))
            {
                return _serialization[typeof (String)](constant.Value.ToString());
            }

            return _serialization[constantType](constant.Value);
        }
    }
}