using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     Declares interface of LINQ Exression translator.
    /// </summary>
    public interface IQueryTranslator
    {
        /// <summary>
        ///     Translates an expression into collection of query segments.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <returns>Collection of query segments.</returns>
        IDictionary<QuerySegment, string> Translate(Expression expression);
    }
}