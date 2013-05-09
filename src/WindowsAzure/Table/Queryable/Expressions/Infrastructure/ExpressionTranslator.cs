using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Infrastructure
{
    /// <summary>
    ///     Expression translator.
    ///     http://msdn.microsoft.com/en-us/library/windowsazure/dd894031.aspx
    /// </summary>
    internal sealed class ExpressionTranslator : ExpressionVisitor
    {
        private readonly ExpressionEvaluator _constantEvaluator;
        private readonly IDictionary<string, string> _nameChanges;
        private StringBuilder _filter;
        private ITranslationResult _result;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="nameChanges"></param>
        internal ExpressionTranslator(IDictionary<string, string> nameChanges)
        {
            _nameChanges = nameChanges;
            _constantEvaluator = new ExpressionEvaluator();
        }

        public void Translate(ITranslationResult result, MethodCallExpression method)
        {
            _result = result;
            _filter = new StringBuilder();

            var lambda = (LambdaExpression) StripQuotes(method.Arguments[1]);

            Visit(lambda.Body);

            AddFilter();
        }

        public void AddPostProcessing(MethodCallExpression method)
        {
            Type type = method.Arguments[0].Type.GetGenericArguments()[0];
            Type genericType = typeof (IQueryable<>).MakeGenericType(type);

            ParameterExpression parameter = Expression.Parameter(genericType, null);
            MethodInfo methodInfo = typeof (System.Linq.Queryable)
                .GetMethods()
                .Single(p => p.Name == method.Method.Name && p.GetParameters().Length == 1)
                .MakeGenericMethod(type);

            MethodCallExpression call = Expression.Call(methodInfo, parameter);

            _result.AddPostProcesing(Expression.Lambda(call, parameter));
        }

        private void AddFilter()
        {
            string filter = RemoveParentheses(_filter.ToString().Trim());
            _result.AddFilter(filter);
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

        public static Expression StripQuotes(Expression expression)
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
                string message = String.Format(Resources.TranslatorOperatorNotSupported, unary.NodeType);
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

            AppendBinaryExpression(binary);

            if (paranthesesRequired)
            {
                _filter.Append(")");
            }

            return binary;
        }

        private void AppendBinaryExpression(BinaryExpression binary)
        {
            // Left part
            if (binary.Left.NodeType == ExpressionType.Call)
            {
                if (AppendBinaryCall((MethodCallExpression) binary.Left, binary.NodeType))
                {
                    return;
                }
            }
            else
            {
                AppendBinaryPart(binary.Left);
            }

            // Comparison
            if (!binary.NodeType.IsSupported())
            {
                string message = String.Format(Resources.TranslatorOperatorNotSupported, binary.NodeType);
                throw new NotSupportedException(message);
            }

            _filter.AppendFormat(" {0} ", binary.NodeType.Serialize());

            // Right part
            if (binary.Right.NodeType == ExpressionType.Call)
            {
                var methodCall = (MethodCallExpression) binary.Right;

                if (AppendBinaryCall(methodCall, binary.NodeType))
                {
                    string message = String.Format(Resources.TranslatorMethodNotSupported, methodCall.Method.Name);
                    throw new ArgumentException(message);
                }
            }
            else
            {
                AppendBinaryPart(binary.Right);
            }
        }

        private void AppendBinaryPart(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Invoke)
            {
                var invocation = (InvocationExpression) expression;
                Visit(invocation.Expression);
                return;
            }

            // Check whether expression is binary
            if (expression.NodeType.IsSupported())
            {
                Visit(expression);
            }
            else
            {
                AppendConstant(_constantEvaluator.Evaluate(expression));
            }
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            switch (node.Method.Name)
            {
                case "Contains":
                    if (node.Arguments.Count == 1)
                    {
                        var result = (ConstantExpression) _constantEvaluator.Evaluate(node.Object);

                        var enumerable = result.Value as IEnumerable;
                        if (enumerable == null)
                        {
                            string message = string.Format(Resources.TranslatorMethodInvalidArgument, node.Method.Name);
                            throw new ArgumentException(message);
                        }

                        Expression parameter = node.Arguments[0];

                        // determine equality value
                        string equality;

                        if (_filter.Length >= 5 &&
                            _filter.ToString(_filter.Length - 5, 5) == " not ")
                        {
                            _filter.Remove(_filter.Length - 5, 5);
                            equality = "ne";
                        }
                        else
                        {
                            equality = "eq";
                        }

                        _filter.Append("(");
                        int count = 0;

                        foreach (object value in enumerable)
                        {
                            AppendConstant(parameter);
                            _filter.AppendFormat(" {0} ", equality);
                            AppendConstant(Expression.Constant(value));
                            _filter.Append(" or ");
                            count++;
                        }

                        if (count > 0)
                        {
                            _filter.Remove(_filter.Length - 4, 4);
                            _filter.Append(")");
                        }
                        else
                        {
                            _filter.Remove(_filter.Length - 1, 1);
                        }
                    }
                    else
                    {
                        string message = string.Format(Resources.TranslatorMethodInvalidArgument, node.Method.Name);
                        throw new ArgumentException(message);
                    }
                    break;

                case "ToString":
                    AppendConstant(Expression.Constant(node.Object != null ? node.Object.ToString() : string.Empty));
                    break;

                default:
                    {
                        string message = string.Format(Resources.TranslatorMethodNotSupported, node.Method.Name);
                        throw new ArgumentException(message);
                    }
            }

            return node;
        }

        /// <summary>
        ///     Translates method call expression.
        /// </summary>
        /// <param name="node">Expression.</param>
        /// <param name="type">Expression type.</param>
        /// <returns>Whether expression has been completely translated.</returns>
        private bool AppendBinaryCall(MethodCallExpression node, ExpressionType type)
        {
            switch (node.Method.Name)
            {
                case "CompareTo":
                    AppendConstant(_constantEvaluator.Evaluate(node.Object));
                    _filter.AppendFormat(" {0} ", type.Serialize());
                    AppendConstant(_constantEvaluator.Evaluate(node.Arguments[0]));
                    return true;

                case "Compare":
                case "CompareOrdinal":
                    if (node.Arguments.Count >= 2)
                    {
                        AppendConstant(_constantEvaluator.Evaluate(node.Arguments[0]));
                        _filter.AppendFormat(" {0} ", type.Serialize());
                        AppendConstant(_constantEvaluator.Evaluate(node.Arguments[1]));
                    }
                    else
                    {
                        string message = string.Format(Resources.TranslatorMethodInvalidArgument, node.Method.Name);
                        throw new ArgumentException(message);
                    }
                    return true;

                default:
                    VisitMethodCall(node);
                    break;
            }

            return false;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            AppendConstant(node);

            return node;
        }

        private void AppendConstant(Expression node)
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
                    string name = member.Member.Name;
                    _filter.Append(_nameChanges.ContainsKey(name) ? _nameChanges[name] : name);

                    return;
                }

                string message = String.Format(Resources.TranslatorMemberNotSupported, member.Member.Name);
                throw new NotSupportedException(message);
            }

            if (node.NodeType != ExpressionType.Constant)
            {
                string message = String.Format(Resources.TranslatorUnableToEvaluateExpression, node);
                throw new InvalidExpressionException(message);
            }

            var constant = (ConstantExpression) node;
            _filter.Append(constant.Serialize());
        }
    }
}