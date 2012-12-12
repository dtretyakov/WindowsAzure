using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators.Methods;

namespace GitHub.WindowsAzure.Table.Queryable.ExpressionTranslators
{
    public class QueryTranslator : ExpressionVisitor, IQueryTranslator
    {
        private readonly IDictionary<string, IMethodTranslator> _methodTranslators;
        private readonly Dictionary<string, string> _result;

        public QueryTranslator(IDictionary<string, IMethodTranslator> methodTranslators)
        {
            _methodTranslators = methodTranslators;
            _result = new Dictionary<string, string>();
        }

        public IDictionary<string, string> Translate(Expression expression)
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

                if (!_methodTranslators.ContainsKey(methodName))
                {
                    throw new NotSupportedException(
                        string.Format("The method '{0}' is not supported", methodName));
                }

                _result.Add(methodName, _methodTranslators[methodName].Translate(m));

                Visit(m.Arguments[0]);
            }

            return m;
        }
    }
}