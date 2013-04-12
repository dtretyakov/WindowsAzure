using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Where expression translator.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx
    /// </summary>
    public class WhereTranslator : ExpressionVisitor, IMethodTranslator
    {
        private static readonly List<String> SupportedMethods;
        private readonly ExpressionEvaluator _constantEvaluator;
        private StringBuilder _filter;
        private IDictionary<String, String> _nameMappings;

        static WhereTranslator()
        {
            SupportedMethods = new List<string>
                {
                    "Where",
                    "First",
                    "FirstOrDefault",
                    "Single",
                    "SingleOrDefault"
                };
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        public WhereTranslator()
        {
            _constantEvaluator = new ExpressionEvaluator();
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
            get { return SupportedMethods; }
        }

        private static String RemoveParentheses(string filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            while (filter.Length > 2 && filter[0] == '(' && filter[filter.Length - 1] == ')')
            {
                filter = filter.Substring(1, filter.Length - 2);
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
            if (!unary.NodeType.IsSupported())
            {
                string message = String.Format(Resources.WhereTranslatorOperatorNotSupported, unary.NodeType);
                throw new NotSupportedException(message);
            }

            _filter.AppendFormat(" {0} ", unary.NodeType.Serialize());

            Visit(unary.Operand);

            return unary;
        }

        protected override Expression VisitBinary(BinaryExpression binary)
        {
            bool paranthesesRequired = binary.NodeType.IsSupported() && (binary.Left.NodeType.IsSupported() || binary.Right.NodeType.IsSupported());

            if (paranthesesRequired)
            {
                _filter.Append("(");
            }

            VisitBinaryLeft(binary);

            if (!binary.NodeType.IsSupported())
            {
                string message = String.Format(Resources.WhereTranslatorOperatorNotSupported, binary.NodeType);
                throw new NotSupportedException(message);
            }

            _filter.AppendFormat(" {0} ", binary.NodeType.Serialize());

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

            if (binary.Left.NodeType.IsSupported())
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

            if (binary.Right.NodeType.IsSupported())
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

                string message = String.Format(Resources.WhereTranslatorMemberNotSupported, member.Member.Name);
                throw new NotSupportedException(message);
            }

            if (node.NodeType != ExpressionType.Constant)
            {
                string message = String.Format(Resources.WhereTranslatorUnableToEvaluateExpression, node);
                throw new InvalidExpressionException(message);
            }

            var constant = (ConstantExpression) node;
            _filter.Append(constant.Serialize());
        }
    }
}