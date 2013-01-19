using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public QueryTranslator(IDictionary<String, String> nameChanges,
                               IList<IMethodTranslator> methodTranslators)
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
            expression = Evaluator.PartialEval(expression);

            Visit(expression);

            return _result;
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            if (m.Method.DeclaringType == typeof (System.Linq.Queryable))
            {
                string methodName = m.Method.Name;

                // Select method translator
                IMethodTranslator translator = _methodTranslators.FirstOrDefault(
                    p => p.AcceptedMethods.Contains(methodName));

                if (translator == null)
                {
                    throw new NotSupportedException(
                        string.Format("The method '{0}' is not supported", methodName));
                }

                // Merge translation results
                foreach (var result in translator.Translate(m, _nameChanges))
                {
                    _result.Add(result.Key, result.Value);
                }

                Visit(m.Arguments[0]);
            }

            return m;
        }
    }
}