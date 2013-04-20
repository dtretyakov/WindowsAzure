using System;
using System.Linq.Expressions;
using System.Reflection;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Performs evaluation of the LINQ Expression.
    /// </summary>
    public sealed class ExpressionEvaluator : ExpressionVisitor
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
                case ExpressionType.MemberAccess:
                    {
                        return GetMemberConstant(node);
                    }

                case ExpressionType.Constant:
                    {
                        return Expression.Constant(GetFieldValue(node), node.Type);
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
            var innerField = (FieldInfo) node.Member;
            object innerObj = null;

            if (node.Expression != null)
            {
                var ce = (ConstantExpression) Evaluate(node.Expression);
                innerObj = ce.Value;
            }

            return innerField.GetValue(innerObj);
        }

        private object GetPropertyValue(MemberExpression node)
        {
            var outerProp = (PropertyInfo) node.Member;
            var innerMember = (MemberExpression) node.Expression;
            var innerField = (FieldInfo) innerMember.Member;
            var ce = (ConstantExpression) Evaluate(innerMember.Expression);
            object innerObj = ce.Value;
            object outerObj = innerField.GetValue(innerObj);

            return outerProp.GetValue(outerObj, null);
        }
    }
}