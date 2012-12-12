using System.Linq.Expressions;

namespace GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators.Methods
{
    /// <summary>
    ///     Expression method translator.
    /// </summary>
    public interface IMethodTranslator
    {
        /// <summary>
        ///     Provides evaluated query information.
        /// </summary>
        /// <param name="method">Expression method.</param>
        /// <returns>Result.</returns>
        string Translate(MethodCallExpression method);
    }
}