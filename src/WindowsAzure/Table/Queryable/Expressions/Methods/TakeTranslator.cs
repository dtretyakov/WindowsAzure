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

        public void Translate(MethodCallExpression method, ITranslationResult result)
        {
            if (method.Method.Name != MethodName || method.Arguments.Count != 2)
            {
                var message = string.Format(Resources.TranslatorMemberNotSupported, method.NodeType);
                throw new ArgumentOutOfRangeException("method", message);
            }

            var constant = (ConstantExpression) method.Arguments[1];

            result.AddTop((int) constant.Value);
        }
    }
}