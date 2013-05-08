using System;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Infrastructure
{
    /// <summary>
    ///     Performs evaluation of the LINQ Expression.
    /// </summary>
    internal sealed class ExpressionEvaluator : ExpressionVisitor
    {
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
                return GetMemberConstant(node);
            }

            switch (node.Expression.NodeType)
            {
                case ExpressionType.Constant:
                case ExpressionType.MemberAccess:
                    {
                        return GetMemberConstant(node);
                    }

                default:
                    {
                        return base.VisitMember(node);
                    }
            }
        }

        private ConstantExpression GetMemberConstant(MemberExpression node)
        {
            object value;

            if (node.Member.MemberType == MemberTypes.Field)
            {
                value = GetFieldValue(node);
            }
            else if (node.Member.MemberType == MemberTypes.Property)
            {
                value = GetPropertyValue(node);
            }
            else
            {
                throw new NotSupportedException(string.Format("Invalid member type: {0}", node.Member.MemberType));
            }

            return Expression.Constant(value, node.Type);
        }

        private object GetFieldValue(MemberExpression node)
        {
            var fieldInfo = (FieldInfo) node.Member;
            object @object = null;

            if (node.Expression != null)
            {
                var ce = (ConstantExpression) Evaluate(node.Expression);
                @object = ce.Value;
            }

            return fieldInfo.GetValue(@object);
        }

        private object GetPropertyValue(MemberExpression node)
        {
            var propertyInfo = (PropertyInfo) node.Member;
            object @object = null;

            if (node.Expression != null)
            {
                var ce = (ConstantExpression) Evaluate(node.Expression);
                @object = ce.Value;
            }

            return propertyInfo.GetValue(@object, null);
        }
    }
}