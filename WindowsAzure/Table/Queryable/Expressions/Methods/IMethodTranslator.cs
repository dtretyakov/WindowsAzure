using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Expression method translator.
    /// </summary>
    internal interface IMethodTranslator
    {
        /// <summary>
        ///     Gets a method name.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Provides evaluated query information.
        /// </summary>
        /// <param name="method">Expression method.</param>
        /// <param name="result">Translation result.</param>
        void Translate(MethodCallExpression method, ITranslationResult result);
    }
}