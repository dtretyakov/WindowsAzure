using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Expression method translator.
    /// </summary>
    internal interface IMethodTranslator
    {
        /// <summary>
        ///     Gets a list of accepted methods.
        /// </summary>
        IList<String> AcceptedMethods { get; }

        /// <summary>
        ///     Provides evaluated query information.
        /// </summary>
        /// <param name="result">Translation result.</param>
        /// <param name="method">Expression method.</param>
        void Translate(ITranslationResult result, MethodCallExpression method);
    }
}