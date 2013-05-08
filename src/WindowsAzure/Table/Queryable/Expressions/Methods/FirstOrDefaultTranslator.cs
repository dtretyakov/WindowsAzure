using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ FirstOrDefault method translator.
    /// </summary>
    internal sealed class FirstOrDefaultTranslator : MethodTranslatorBase
    {
        private const string MethodName = "FirstOrDefault";

        public FirstOrDefaultTranslator(IDictionary<string, string> nameChanges)
            : base(nameChanges)
        {
        }

        public override bool CanTranslate(MethodCallExpression method)
        {
            return method.Method.Name == MethodName;
        }
    }
}