using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Take expression translator.
    /// </summary>
    public class TakeTranslator : ExpressionVisitor, IMethodTranslator
    {
        private static readonly List<String> SupportedMethods = new List<string> {"Take"};
        private String _takeCount;

        /// <summary>
        ///     Gets a query segment name.
        /// </summary>
        public QuerySegment QuerySegment
        {
            get { return QuerySegment.Top; }
        }

        public IDictionary<QuerySegment, String> Translate(MethodCallExpression method, IDictionary<string, string> nameChanges)
        {
            Visit(method.Arguments[1]);

            return new Dictionary<QuerySegment, string>
                {
                    {QuerySegment.Top, _takeCount}
                };
        }

        public IList<string> AcceptedMethods
        {
            get { return SupportedMethods; }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _takeCount = node.Value.ToString();

            return base.VisitConstant(node);
        }
    }
}