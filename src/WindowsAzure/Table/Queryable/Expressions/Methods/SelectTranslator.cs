using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Select expression translator.
    /// </summary>
    public class SelectTranslator : ExpressionVisitor, IMethodTranslator
    {
        private readonly List<String> _acceptedMethods;
        private readonly List<String> _columns;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public SelectTranslator()
        {
            _acceptedMethods = new List<string> {"Select"};
            _columns = new List<string>();
        }

        /// <summary>
        ///     Gets a query segment name.
        /// </summary>
        public QuerySegment QuerySegment
        {
            get { return QuerySegment.Select; }
        }

        public IDictionary<QuerySegment, String> Translate(MethodCallExpression method, IDictionary<string, string> nameChanges)
        {
            Visit(method);

            return new Dictionary<QuerySegment, string>
                {
                    {QuerySegment.Select, String.Join(",", _columns)}
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