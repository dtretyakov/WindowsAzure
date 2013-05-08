using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Expression method translator.
    /// </summary>
    internal interface IMethodTranslator
    {
        /// <summary>
        ///     Determines whether method call can be translated.
        /// </summary>
        /// <param name="method">Expression method.</param>
        /// <returns>Result whether method can be translated.</returns>
        bool CanTranslate(MethodCallExpression method);

        /// <summary>
        ///     Provides evaluated query information.
        /// </summary>
        /// <param name="methodCall">Expression method.</param>
        /// <param name="result">Translation result.</param>
        void Translate(MethodCallExpression methodCall, ITranslationResult result);
    }
}