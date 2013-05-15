using System.Collections.Generic;

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