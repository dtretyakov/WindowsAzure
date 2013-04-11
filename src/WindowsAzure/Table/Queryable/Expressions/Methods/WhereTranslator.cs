using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Where expression translator.
    /// </summary>
    public class WhereTranslator : ExpressionVisitor, IMethodTranslator
    {
        private readonly List<String> _acceptedMethods;
        private readonly ConstantEvaluator _constantEvaluator;
        private readonly ConstrantSerializer _constrantSerializer;

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


        private StringBuilder _filter;
        private IDictionary<String, String> _nameMappings;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public WhereTranslator()
        {
            _constantEvaluator = new ConstantEvaluator();
            _constrantSerializer = new ConstrantSerializer();

            _acceptedMethods = new List<string>
                {
                    "Where",
                    "First",
                    "FirstOrDefault",
                    "Single",
                    "SingleOrDefault"
                };
        }

        /// <summary>
        ///     Gets a query segment name.
        /// </summary>
        public QuerySegment QuerySegment
        {
            get { return QuerySegment.Filter; }
        }

        public IDictionary<QuerySegment, String> Translate(MethodCallExpression method, IDictionary<string, string> nameChanges)
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

        private static Expression StripQuotes(Expression expression)
        {
            while (expression.NodeType == ExpressionType.Quote)
            {
                expression = ((UnaryExpression) expression).Operand;
            }

            return expression;
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

            VisitBinaryLeft(binary);

            if (!_logicalOperators.ContainsKey(binary.NodeType))
            {
                string message = String.Format("The binary operator '{0}' is not supported", binary.NodeType);
                throw new NotSupportedException(message);
            }

            _filter.AppendFormat(" {0} ", _logicalOperators[binary.NodeType]);

            VisitBinaryRight(binary);

            if (paranthesesRequired)
            {
                _filter.Append(")");
            }

            return binary;
        }

        protected virtual void VisitBinaryLeft(BinaryExpression binary)
        {
            if (binary.Left.NodeType == ExpressionType.Call)
            {
                var method = (MethodCallExpression) binary.Left;

                switch (method.Method.Name)
                {
                    case "CompareTo":
                        if (method.Object != null && method.Object.Type == typeof (String))
                        {
                            AppendConstrant(_constantEvaluator.Evaluate(method.Object));
                            return;
                        }
                        break;

                    case "Compare":
                    case "CompareOrdinal":
                        if (method.Arguments.Count >= 2)
                        {
                            AppendConstrant(_constantEvaluator.Evaluate(method.Arguments[0]));
                            return;
                        }
                        break;
                }

                AppendConstrant(_constantEvaluator.Evaluate(method.Arguments[0]));
            }

            if (_logicalOperators.ContainsKey(binary.Left.NodeType))
            {
                Visit(binary.Left);
                return;
            }

            AppendConstrant(_constantEvaluator.Evaluate(binary.Left));
        }

        protected virtual void VisitBinaryRight(BinaryExpression binary)
        {
            if (binary.Left.NodeType == ExpressionType.Call)
            {
                var method = (MethodCallExpression) binary.Left;

                switch (method.Method.Name)
                {
                    case "CompareTo":
                        AppendConstrant(_constantEvaluator.Evaluate(method.Arguments[0]));
                        return;

                    case "Compare":
                    case "CompareOrdinal":
                        AppendConstrant(_constantEvaluator.Evaluate(method.Arguments[1]));
                        return;
                }

                AppendConstrant(_constantEvaluator.Evaluate(method.Arguments[0]));
            }

            if (_logicalOperators.ContainsKey(binary.Right.NodeType))
            {
                Visit(binary.Right);
                return;
            }

            AppendConstrant(_constantEvaluator.Evaluate(binary.Right));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            AppendConstrant(node);

            return base.VisitMember(node);
        }

        private void AppendConstrant(Expression node)
        {
            if (node.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression) node;

                if (member.Expression == null || member.Expression.NodeType != ExpressionType.Parameter)
                {
                    node = _constantEvaluator.Evaluate(member);
                }
            }

            if (node.NodeType == ExpressionType.MemberAccess)
            {
                var member = (MemberExpression) node;

                if (member.Expression != null && member.Expression.NodeType == ExpressionType.Parameter)
                {
                    _filter.Append(_nameMappings.ContainsKey(member.Member.Name)
                                       ? _nameMappings[member.Member.Name]
                                       : member.Member.Name);

                    return;
                }

                string message = String.Format("The member '{0}' is not supported", member.Member.Name);
                throw new NotSupportedException(message);
            }

            if (node.NodeType != ExpressionType.Constant)
            {
                string message = String.Format("Unable to evaluate expression: {0}", node);
                throw new InvalidExpressionException(message);
            }

            _filter.Append(_constrantSerializer.Serialize((ConstantExpression) node));
        }
    }
}