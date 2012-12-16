using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.ExpressionTranslators.Methods;

namespace WindowsAzure.Table.Queryable.ExpressionTranslators
{
    public class QueryTranslator : ExpressionVisitor, IQueryTranslator
    {
        private readonly IList<IMethodTranslator> _methodTranslators;
        private readonly IDictionary<string, string> _nameMappings;
        private readonly Dictionary<QueryConstants, string> _result;

        public QueryTranslator(IDictionary<String, String> nameMappings,
                               IList<IMethodTranslator> methodTranslators)
        {
            _nameMappings = nameMappings;
            _methodTranslators = methodTranslators;
            _result = new Dictionary<QueryConstants, string>();
        }

        public IDictionary<QueryConstants, string> Translate(Expression expression)
        {
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
                foreach (var result in translator.Translate(m, _nameMappings))
                {
                    _result.Add(result.Key, result.Value);
                }

                Visit(m.Arguments[0]);
            }

            return m;
        }
    }
}