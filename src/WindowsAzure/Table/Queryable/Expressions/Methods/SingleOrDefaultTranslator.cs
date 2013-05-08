using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ SingleOrDefault method translator.
    /// </summary>
    internal sealed class SingleOrDefaultTranslator : MethodTranslatorBase
    {
        private const string MethodName = "SingleOrDefault";

        public SingleOrDefaultTranslator(IDictionary<string, string> nameChanges)
            : base(nameChanges)
        {
        }

        public override bool CanTranslate(MethodCallExpression method)
        {
            return method.Method.Name == MethodName;
        }
    }
}