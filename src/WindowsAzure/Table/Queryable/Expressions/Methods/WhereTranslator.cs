using System.Collections.Generic;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Infrastructure;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ Where method translator.
    /// </summary>
    internal sealed class WhereTranslator : IMethodTranslator
    {
        private const string MethodName = "Where";
        private readonly IDictionary<string, string> _nameChanges;

        public WhereTranslator(IDictionary<string, string> nameChanges)
        {
            _nameChanges = nameChanges;
        }

        public string Name
        {
            get { return MethodName; }
        }

        public void Translate(MethodCallExpression method, ITranslationResult result)
        {
            var expressionTranslator = new ExpressionTranslator(_nameChanges);
            expressionTranslator.Translate(result, method);
        }
    }
}