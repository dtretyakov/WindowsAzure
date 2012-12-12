using System.Collections.Generic;
using System.Linq.Expressions;

namespace GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators
{
    public interface IQueryTranslator
    {
        IDictionary<string, string> Translate(Expression expression);
    }
}