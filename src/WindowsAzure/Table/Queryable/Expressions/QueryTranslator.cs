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
    public class QueryTranslator : ExpressionVisitor, IQueryTranslator
    {
        private readonly IList<IMethodTranslator> _methodTranslators;
        private readonly IDictionary<string, string> _nameChanges;
        private Dictionary<QuerySegment, string> _result;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="nameChanges">Entity name mappings.</param>
        public QueryTranslator(IDictionary<String, String> nameChanges)
            : this(nameChanges, new List<IMethodTranslator>
                {
                    new SelectTranslator(),
                    new TakeTranslator(),
                    new WhereTranslator()
                })
        {
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="nameChanges">Entity name mappings.</param>
        /// <param name="methodTranslators">LINQ Expression methods translators.</param>
        public QueryTranslator(IDictionary<String, String> nameChanges, IList<IMethodTranslator> methodTranslators)
        {
            _nameChanges = nameChanges;
            _methodTranslators = methodTranslators;
        }

        /// <summary>
        ///     Translates an LINQ expression into collection of query segments.
        /// </summary>
        /// <param name="expression">LINQ expression.</param>
        /// <returns>Collection of query segments.</returns>
        public IDictionary<QuerySegment, string> Translate(Expression expression)
        {
            _result = new Dictionary<QuerySegment, string>();

            Visit(expression);

            return _result;
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

                // Merge translation results
                foreach (var result in translator.Translate(methodCall, _nameChanges))
                {
                    _result.Add(result.Key, result.Value);
                }

                Visit(methodCall.Arguments[0]);
            }

            return methodCall;
        }
    }
}