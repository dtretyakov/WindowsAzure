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
        private readonly List<String> _acceptedMethods;
        private String _takeCount;

        public TakeTranslator()
        {
            _acceptedMethods = new List<string> {"Take"};
        }

        public QuerySegment QuerySegment
        {
            get { return QuerySegment.Top; }
        }

        public IDictionary<QuerySegment, String> Translate(
            MethodCallExpression method,
            IDictionary<string, string> nameChanges)
        {
            Visit(method.Arguments[1]);

            return new Dictionary<QuerySegment, string>
                       {
                           {QuerySegment.Top, _takeCount}
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