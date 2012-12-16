using System.Collections.Generic;
using System.Linq.Expressions;

namespace GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators.Methods
{
    /// <summary>
    ///     Linq Select expression translator.
    /// </summary>
    public class SelectTranslator : ExpressionVisitor, IMethodTranslator
    {
        private readonly List<string> _columns = new List<string>();

        public string Translate(MethodCallExpression method)
        {
            Visit(method);

            return string.Join(",", _columns);
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