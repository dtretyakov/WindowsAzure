using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Xml;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Where expression translator.
    /// </summary>
    public class WhereTranslator : ExpressionVisitor, IMethodTranslator
    {
        private readonly List<String> _acceptedMethods;

        /// <summary>
        ///     Collection of supported logical operands.
        /// </summary>
        private readonly Dictionary<ExpressionType, String> _logicalOperators =
            new Dictionary<ExpressionType, String>
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
                    {typeof (Int32), o => o.ToString()},
                    {typeof (Int64), o => string.Format("{0}L", o)},
                    {typeof (Single), o => string.Format("{0:#.0#}", o)},
                    {typeof (Double), o => string.Format("{0:#.0#}", o)},
                    {typeof (Guid), o => string.Format("guid'{0}'", o)},
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

        private StringBuilder _filter;
        private IDictionary<String, String> _nameMappings;

        public WhereTranslator()
        {
            _acceptedMethods = new List<string>
                                   {
                                       "Where",
                                       "First",
                                       "FirstOrDefault",
                                       "Single",
                                       "SingleOrDefault"
                                   };
        }

        public QuerySegment QuerySegment
        {
            get { return QuerySegment.Filter; }
        }

        public IDictionary<QuerySegment, String> Translate(
            MethodCallExpression method,
            IDictionary<string, string> nameChanges)
        {
            _nameMappings = nameChanges;

            _filter = new StringBuilder();

            var lambda = (LambdaExpression) StripQuotes(method.Arguments[1]);

            Visit(lambda.Body);

            return new Dictionary<QuerySegment, String>
                       {
                           {QuerySegment.Filter, RemoveParentheses(_filter.ToString())}
                       };
        }

        public IList<string> AcceptedMethods
        {
            get { return _acceptedMethods; }
        }

        private static String RemoveParentheses(string filter)
        {
            if (filter.Length < 2)
            {
                throw new ArgumentOutOfRangeException("filter");
            }

            if (filter[0] == '(' && filter[filter.Length - 1] == ')')
            {
                return RemoveParentheses(filter.Substring(1, filter.Length - 2));
            }

            return filter;
        }

        private static Expression StripQuotes(Expression e)
        {
            while (e.NodeType == ExpressionType.Quote)
            {
                e = ((UnaryExpression) e).Operand;
            }

            return e;
        }

        protected override Expression VisitUnary(UnaryExpression unary)
        {
            if (!_logicalOperators.ContainsKey(unary.NodeType))
            {
                throw new NotSupportedException(
                    String.Format("The binary operator '{0}' is not supported", unary.NodeType));
            }

            _filter.AppendFormat(" {0} ", _logicalOperators[unary.NodeType]);

            Visit(unary.Operand);

            return unary;
        }

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            bool paranthesesRequired = _logicalOperators.ContainsKey(binary.NodeType) &&
                                       (_logicalOperators.ContainsKey(binary.Left.NodeType) ||
                                        _logicalOperators.ContainsKey(binary.Right.NodeType));

            if (paranthesesRequired)
            {
                _filter.Append("(");
            }

            Visit(binary.Left);

            if (!_logicalOperators.ContainsKey(binary.NodeType))
            {
                throw new NotSupportedException(
                    String.Format("The binary operator '{0}' is not supported", binary.NodeType));
            }

            _filter.AppendFormat(" {0} ", _logicalOperators[binary.NodeType]);

            Visit(binary.Right);

            if (paranthesesRequired)
            {
                _filter.Append(")");
            }

            return binary;
        }

        protected override Expression VisitConstant(ConstantExpression constant)
        {
            if (constant.Value == null)
            {
                _filter.Append("null");
            }
            else
            {
                Type constantType = constant.Value.GetType();

                if (!_serialization.ContainsKey(constantType))
                {
                    throw new NotSupportedException(
                        String.Format("The constant for '{0}' is not supported", constant.Value));
                }

                _filter.Append(_serialization[constantType](constant.Value));
            }

            return constant;
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            if (member.Expression != null && member.Expression.NodeType == ExpressionType.Parameter)
            {
                _filter.Append(_nameMappings.ContainsKey(member.Member.Name)
                                   ? _nameMappings[member.Member.Name]
                                   : member.Member.Name);

                return member;
            }

            throw new NotSupportedException(
                String.Format("The member '{0}' is not supported", member.Member.Name));
        }
    }
}