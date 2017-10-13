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
    internal class QueryTranslator : IQueryTranslator
    {
        private readonly IDictionary<string, IMethodTranslator> _methodTranslators;

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
        internal QueryTranslator(IEnumerable<IMethodTranslator> methodTranslators)
        {
            _methodTranslators = methodTranslators.ToDictionary(translator => translator.Name);
        }

        /// <summary>
        ///     Translates a LINQ expression into collection of query segments.
        /// </summary>
        /// <param name="expression">LINQ expression.</param>
        /// <param name="result">Translation result.</param>
        /// <returns>Collection of query segments.</returns>
        public void Translate(Expression expression, ITranslationResult result)
        {
            if (expression.NodeType != ExpressionType.Call)
            {
                return;
            }

            var methodCall = (MethodCallExpression) expression;

            // Visit method
            VisitMethodCall(methodCall, result);

            // ReSharper disable ForCanBeConvertedToForeach

            // Visit arguments
            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                Expression argument = methodCall.Arguments[i];
                if (argument.NodeType == ExpressionType.Call)
                {
                    Translate(argument, result);
                }
            }

            // ReSharper restore ForCanBeConvertedToForeach
        }

        private static IEnumerable<IMethodTranslator> GetTranslators(IDictionary<string, string> nameChanges)
        {
            return new List<IMethodTranslator>
                {
                    new WhereTranslator(nameChanges),
                    new FirstTranslator(nameChanges),
                    new FirstOrDefaultTranslator(nameChanges),
                    new SingleTranslator(nameChanges),
                    new SingleOrDefaultTranslator(nameChanges),
                    new SelectTranslator(nameChanges),
                    new TakeTranslator()
                };
        }

        private void VisitMethodCall(MethodCallExpression methodCall, ITranslationResult result)
        {
            if (methodCall.Method.DeclaringType != typeof (System.Linq.Queryable))
            {
                throw new NotSupportedException(string.Format(Resources.TranslatorMethodNotSupported, methodCall.Method.Name));
            }

            // Get a method translator
            IMethodTranslator translator;

            if (!_methodTranslators.TryGetValue(methodCall.Method.Name, out translator))
            {
                string message = String.Format(Resources.TranslatorMethodNotSupported, methodCall.Method.Name);
                throw new NotSupportedException(message);
            }

            translator.Translate(methodCall, result);
        }
    }
}