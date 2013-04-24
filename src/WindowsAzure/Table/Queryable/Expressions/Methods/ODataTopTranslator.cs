using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     Take expression translator.
    /// </summary>
    internal sealed class ODataTopTranslator : ExpressionVisitor, IMethodTranslator
    {
        private static readonly List<String> SupportedMethods = new List<string> {"Take"};
        private ITranslationResult _result;

        public void Translate(ITranslationResult result, MethodCallExpression method)
        {
            _result = result;

            Visit(method.Arguments[1]);
        }

        public IList<string> AcceptedMethods
        {
            get { return SupportedMethods; }
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _result.AddTop((int) node.Value);
            return base.VisitConstant(node);
        }
    }
}