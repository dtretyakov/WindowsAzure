using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Performs evaluation of the LINQ Expression.
    /// </summary>
    public sealed class ExpressionEvaluator : ExpressionVisitor
    {
        private static readonly ConcurrentDictionary<Expression, Func<object>> CachedMembers;

        static ExpressionEvaluator()
        {
            CachedMembers = new ConcurrentDictionary<Expression, Func<object>>();
        }

        /// <summary>
        ///     Evaluates an expression.
        /// </summary>
        /// <param name="expression">Source expression.</param>
        /// <returns>Evaluated expression.</returns>
        public Expression Evaluate(Expression expression)
        {
            return Visit(expression);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var arguments = new object[node.Arguments.Count];

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var result = (ConstantExpression) Evaluate(node.Arguments[i]);
                arguments[i] = result.Value;
            }

            return Expression.Constant(node.Constructor.Invoke(arguments), node.Type);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            if (node.Type != typeof (Byte[]))
            {
                String message = String.Format(Resources.ExpressionEvaluatorTypeNotSupported, node.Type);
                throw new NotSupportedException(message);
            }

            var array = new Byte[node.Expressions.Count];

            for (int i = 0; i < node.Expressions.Count; i++)
            {
                var result = (ConstantExpression) Evaluate(node.Expressions[i]);
                array[i] = (Byte) result.Value;
            }

            return Expression.Constant(array, node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var arguments = new object[node.Arguments.Count];

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var result = (ConstantExpression) Evaluate(node.Arguments[i]);
                arguments[i] = result.Value;
            }

            var constantObject = (ConstantExpression) Evaluate(node.Object);
            object value = node.Method.Invoke(constantObject.Value, arguments);

            return Expression.Constant(value, node.Type);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression == null)
            {
                return base.VisitMember(node);
            }

            switch (node.Expression.NodeType)
            {
                case ExpressionType.MemberAccess:
                case ExpressionType.Constant:
                    {
                        Func<object> valueAccessor = CachedMembers.GetOrAdd(node, GetValueAccessor);
                        return Expression.Constant(valueAccessor(), node.Type);
                    }

                default:
                    {
                        return base.VisitMember(node);
                    }
            }
        }

        private static Func<object> GetValueAccessor(Expression node)
        {
            UnaryExpression objectMember = Expression.Convert(node, typeof (object));
            return Expression.Lambda<Func<object>>(objectMember).Compile();
        }
    }
}