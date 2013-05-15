using System.Collections.Generic;

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

        public override void Translate(System.Linq.Expressions.MethodCallExpression methodCall, ITranslationResult result)
        {
            base.Translate(methodCall, result);
            result.AddTop(2);
        }

        public override string Name
        {
            get { return MethodName; }
        }
    }
}