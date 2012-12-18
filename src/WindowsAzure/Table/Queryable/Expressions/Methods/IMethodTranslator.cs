using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Expression method translator.
    /// </summary>
    public interface IMethodTranslator
    {
        /// <summary>
        ///     Gets a list of accepted methods.
        /// </summary>
        IList<String> AcceptedMethods { get; }

        /// <summary>
        ///     Provides evaluated query information.
        /// </summary>
        /// <param name="method">Expression method.</param>
        /// <param name="nameChanges">Property name changes.</param>
        /// <returns>Result.</returns>
        IDictionary<QuerySegment, String> Translate(
            MethodCallExpression method,
            IDictionary<String, String> nameChanges);
    }
}