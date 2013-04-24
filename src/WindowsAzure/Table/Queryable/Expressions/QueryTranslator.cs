using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Properties;
using WindowsAzure.Table.Queryable.Expressions.Methods;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     Manages translation of the LINQ expressions.
    /// </summary>
    internal class QueryTranslator : ExpressionVisitor, IQueryTranslator
    {
        private readonly IList<IMethodTranslator> _methodTranslators;
        private ITranslationResult _result;

        /// <summary>
        ///     Constructor.
        /// </summary>
        internal QueryTranslator(IDictionary<string, string> nameChanges)
            : this(GetTranslators(nameChanges))
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="methodTranslators">LINQ Expression methods translators.</param>
        internal QueryTranslator(IList<IMethodTranslator> methodTranslators)
        {
            _methodTranslators = methodTranslators;
        }

        /// <summary>
        ///     Translates a LINQ expression into collection of query segments.
        /// </summary>
        /// <param name="result">Translation result.</param>
        /// <param name="expression">LINQ expression.</param>
        /// <returns>Collection of query segments.</returns>
        public void Translate(ITranslationResult result, Expression expression)
        {
            _result = result;

            Visit(expression);
        }

        private static IList<IMethodTranslator> GetTranslators(IDictionary<string, string> nameChanges)
        {
            return new List<IMethodTranslator>
                {
                    new ODataFilterTranslator(nameChanges),
                    new ODataSelectTranslator(nameChanges),
                    new ODataTopTranslator()
                };
        }

        protected override Expression VisitMethodCall(MethodCallExpression methodCall)
        {
            if (methodCall.Method.DeclaringType == typeof (System.Linq.Queryable))
            {
                string methodName = methodCall.Method.Name;

                // Select method translator
                IMethodTranslator translator = _methodTranslators.FirstOrDefault(
                    methodTranslator => methodTranslator.AcceptedMethods.Contains(methodName));

                if (translator == null)
                {
                    string message = String.Format(Resources.QueryTranslatorMethodNotSupported, methodName);
                    throw new NotSupportedException(message);
                }

                translator.Translate(_result, methodCall);

                Visit(methodCall.Arguments[0]);
            }

            return base.VisitMethodCall(methodCall);
        }
    }
}