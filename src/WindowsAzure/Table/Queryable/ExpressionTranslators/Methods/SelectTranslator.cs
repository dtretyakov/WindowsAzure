using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.ExpressionTranslators.Methods
{
    /// <summary>
    ///     Linq Select expression translator.
    /// </summary>
    public class SelectTranslator : ExpressionVisitor, IMethodTranslator
    {
        private readonly List<String> _acceptedMethods;
        private readonly List<String> _columns;

        public SelectTranslator()
        {
            _acceptedMethods = new List<string> {"Select"};
            _columns = new List<string>();
        }

        public QueryConstants QuerySegment
        {
            get { return QueryConstants.Select; }
        }

        public IDictionary<QueryConstants, String> Translate(
            MethodCallExpression method,
            IDictionary<string, string> nameMappings)
        {
            Visit(method);

            return new Dictionary<QueryConstants, string>
                       {
                           {QueryConstants.Select, String.Join(",", _columns)}
                       };
        }

        public IList<string> AcceptedMethods
        {
            get { return _acceptedMethods; }
        }

        protected override Expression VisitMember(MemberExpression member)
        {
            if (member.Expression != null && member.Expression.NodeType == ExpressionType.Parameter)
            {
                _columns.Add(member.Member.Name);
            }

            return base.VisitMember(member);
        }
    }
}