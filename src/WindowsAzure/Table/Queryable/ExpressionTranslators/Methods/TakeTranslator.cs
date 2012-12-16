using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.ExpressionTranslators.Methods
{
    /// <summary>
    ///     Linq Take expression translator.
    /// </summary>
    public class TakeTranslator : ExpressionVisitor, IMethodTranslator
    {
        private readonly List<String> _acceptedMethods;
        private String _takeCount;

        public TakeTranslator()
        {
            _acceptedMethods = new List<string> {"Take"};
        }

        public QueryConstants QuerySegment
        {
            get { return QueryConstants.Top; }
        }

        public IDictionary<QueryConstants, String> Translate(MethodCallExpression method,
                                                             IDictionary<string, string> nameMappings)
        {
            Visit(method.Arguments[1]);

            return new Dictionary<QueryConstants, string>
                       {
                           {QueryConstants.Top, _takeCount}
                       };
        }

        public IList<string> AcceptedMethods
        {
            get { return _acceptedMethods; }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _takeCount = node.Value.ToString();

            return base.VisitConstant(node);
        }
    }
}