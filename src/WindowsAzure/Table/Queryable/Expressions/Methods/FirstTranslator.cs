using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ First method translator.
    /// </summary>
    internal sealed class FirstTranslator : MethodTranslatorBase
    {
        private const string MethodName = "First";

        public FirstTranslator(IDictionary<string, string> nameChanges)
            : base(nameChanges)
        {
        }

        public override bool CanTranslate(MethodCallExpression method)
        {
            return method.Method.Name == MethodName;
        }
    }
}