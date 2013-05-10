using System;
using System.Linq.Expressions;
using WindowsAzure.Properties;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ Take method translator.
    /// </summary>
    internal sealed class TakeTranslator : IMethodTranslator
    {
        private const string MethodName = "Take";

        public string Name
        {
            get { return MethodName; }
        }

        public void Translate(MethodCallExpression methodCall, ITranslationResult result)
        {
            if (methodCall.Arguments.Count != 2 || methodCall.Arguments[1].NodeType != ExpressionType.Constant)
            {
                throw new ArgumentException(string.Format(Resources.TranslatorMemberNotSupported, methodCall.NodeType), "methodCall");
            }

            var constant = (ConstantExpression) methodCall.Arguments[1];

            result.AddTop((int) constant.Value);
        }
    }
}