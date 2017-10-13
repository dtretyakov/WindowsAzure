using System.Collections.Generic;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Expressions.Methods
{
    /// <summary>
    ///     LINQ SingleOrDefault method translator.
    /// </summary>
    internal sealed class SingleOrDefaultTranslator : MethodTranslatorBase
    {
        public SingleOrDefaultTranslator(IDictionary<string, string> nameChanges)
            : base(nameChanges, "SingleOrDefault")
        {
        }

        public override void Translate(MethodCallExpression method, ITranslationResult result)
        {
            base.Translate(method, result);
            result.AddTop(2);
        }
    }
}