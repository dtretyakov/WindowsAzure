using System.Linq.Expressions;

namespace GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators.Methods
{
    /// <summary>
    ///     Linq Take expression translator.
    /// </summary>
    public class TakeTranslator : ExpressionVisitor, IMethodTranslator
    {
        private int? _takeCount;

        public string Translate(MethodCallExpression method)
        {
            Visit(method.Arguments[1]);

            return _takeCount.HasValue ? _takeCount.ToString() : null;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value is int)
            {
                _takeCount = (int) node.Value;
            }

            return base.VisitConstant(node);
        }
    }
}