using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ FirstOrDefault method translator.
    /// </summary>
    internal sealed class FirstOrDefaultTranslator : MethodTranslatorBase
    {
        public FirstOrDefaultTranslator(IDictionary<string, string> nameChanges)
            : base(nameChanges, "FirstOrDefault")
        {
        }

        public override void Translate(MethodCallExpression method, ITranslationResult result)
        {
            base.Translate(method, result);
            result.AddTop(1);
        }
    }
}