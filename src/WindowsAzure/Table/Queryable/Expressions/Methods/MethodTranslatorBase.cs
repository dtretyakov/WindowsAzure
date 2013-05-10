using System.Collections.Generic;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Infrastructure;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    internal abstract class MethodTranslatorBase : IMethodTranslator
    {
        private readonly IDictionary<string, string> _nameChanges;

        protected MethodTranslatorBase(IDictionary<string, string> nameChanges)
        {
            _nameChanges = nameChanges;
        }

        public abstract string Name { get; }

        public virtual void Translate(MethodCallExpression methodCall, ITranslationResult result)
        {
            var expressionTranslator = new ExpressionTranslator(_nameChanges);

            MethodCallExpression targetMethod = methodCall;

            if (methodCall.Arguments.Count == 1 && methodCall.Arguments[0].NodeType == ExpressionType.Call)
            {
                targetMethod = (MethodCallExpression) methodCall.Arguments[0];
            }

            expressionTranslator.Translate(result, targetMethod);
            expressionTranslator.AddPostProcessing(methodCall);
        }
    }
}