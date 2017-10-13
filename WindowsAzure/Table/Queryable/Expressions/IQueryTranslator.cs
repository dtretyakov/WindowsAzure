using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     Defines interface of LINQ Exression translator.
    /// </summary>
    internal interface IQueryTranslator
    {
        /// <summary>
        ///     Translates an expression into collection of query segments.
        /// </summary>
        /// <param name="expression">Expression.</param>
        /// <param name="result">Translaion result.</param>
        /// <returns>Table query.</returns>
        void Translate(Expression expression, ITranslationResult result);
    }
}