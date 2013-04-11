using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Performs evaluation of the LINQ Expression.
    /// </summary>
    public sealed class ConstantEvaluator : ExpressionVisitor
    {
        private static readonly ConcurrentDictionary<Expression, Func<object>> CachedMembers;

        static ConstantEvaluator()
        {
            CachedMembers = new ConcurrentDictionary<Expression, Func<object>>();
        }

        /// <summary>
        ///     Evaluates expression.
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
                var result = (ConstantExpression) Visit(node.Arguments[i]);
                arguments[i] = result.Value;
            }

            return Expression.Constant(node.Constructor.Invoke(arguments), node.Type);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            if (node.Type != typeof (Byte[]))
            {
                string message = String.Format("Type {0} is not supported.", node.Type);
                throw new NotSupportedException(message);
            }

            var array = new Byte[node.Expressions.Count];

            for (int i = 0; i < node.Expressions.Count; i++)
            {
                var result = (ConstantExpression) Visit(node.Expressions[i]);
                array[i] = (Byte) result.Value;
            }

            return Expression.Constant(array, node.Type);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var arguments = new object[node.Arguments.Count];

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                var result = (ConstantExpression) Visit(node.Arguments[i]);
                arguments[i] = result.Value;
            }

            return Expression.Constant(node.Method.Invoke(node.Object, arguments), node.Type);
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
                        Func<object> accessor = CachedMembers.GetOrAdd(node.Expression, () =>
                            {
                                UnaryExpression objectMember = Expression.Convert(node, typeof (object));

                                Expression<Func<object>> getterLambda = Expression.Lambda<Func<object>>(objectMember);

                                Func<object> getter = getterLambda.Compile();

                                return getter();
                            });

                        return Expression.Constant(accessor(), node.Type);
                    }

                default:
                    {
                        return base.VisitMember(node);
                    }
            }
        }
    }
}