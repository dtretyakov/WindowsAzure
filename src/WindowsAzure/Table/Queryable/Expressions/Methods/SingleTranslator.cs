using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ Single method translator.
    /// </summary>
    internal sealed class SingleTranslator : MethodTranslatorBase
    {
        private const string MethodName = "Single";

        public SingleTranslator(IDictionary<string, string> nameChanges)
            : base(nameChanges)
        {
        }

        public override bool CanTranslate(MethodCallExpression method)
        {
            return method.Method.Name == MethodName;
        }
    }
}