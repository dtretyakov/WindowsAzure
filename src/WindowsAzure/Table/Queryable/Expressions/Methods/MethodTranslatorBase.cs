using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WindowsAzure.Properties;
using WindowsAzure.Table.Queryable.Expressions.Infrastructure;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    internal abstract class MethodTranslatorBase : IMethodTranslator
    {
        private readonly string _methodName;
        private readonly IDictionary<string, string> _nameChanges;

        protected MethodTranslatorBase(IDictionary<string, string> nameChanges, string methodName)
        {
            _nameChanges = nameChanges;
            _methodName = methodName;
        }

        public string Name
        {
            get { return _methodName; }
        }

        public virtual void Translate(MethodCallExpression method, ITranslationResult result)
        {
            if (method.Method.Name != _methodName)
            {
                string message = string.Format(Resources.TranslatorMethodNotSupported, method.Method.Name);
                throw new ArgumentOutOfRangeException("method", message);
            }

            var expressionTranslator = new ExpressionTranslator(_nameChanges);

            MethodCallExpression targetMethod = method;

            if (method.Arguments.Count == 1 && method.Arguments[0].NodeType == ExpressionType.Call)
            {
                targetMethod = (MethodCallExpression) method.Arguments[0];
            }

            expressionTranslator.Translate(result, targetMethod);
            expressionTranslator.AddPostProcessing(method);
        }
    }
}