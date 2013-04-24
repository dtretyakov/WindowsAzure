using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     Defines interface of OData translation result.
    /// </summary>
    internal interface ITranslationResult
    {
        /// <summary>
        ///     Adds a filter expression.
        /// </summary>
        /// <param name="filter">Filter expression.</param>
        void AddFilter(string filter);

        /// <summary>
        ///     Adds a required count of query elements.
        /// </summary>
        /// <param name="top">Count of elements.</param>
        void AddTop(int top);

        /// <summary>
        ///     Adds a required column name.
        /// </summary>
        /// <param name="column">Column name.</param>
        void AddColumn(string column);

        /// <summary>
        ///     Adds a post processing expression.
        /// </summary>
        /// <param name="lambda">Expression.</param>
        void AddPostProcesing(LambdaExpression lambda);
    }
}