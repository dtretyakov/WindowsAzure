using System.Collections.Generic;

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

        public override void Translate(System.Linq.Expressions.MethodCallExpression methodCall, ITranslationResult result)
        {
            base.Translate(methodCall, result);
            result.AddTop(1);
        }

        public override string Name
        {
            get { return MethodName; }
        }
    }
}