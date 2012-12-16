using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.ExpressionTranslators
{
    public interface IQueryTranslator
    {
        IDictionary<QueryConstants, string> Translate(Expression expression);
    }
}