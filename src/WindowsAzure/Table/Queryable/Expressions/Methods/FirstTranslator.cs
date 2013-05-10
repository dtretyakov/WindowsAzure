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

        public override string Name
        {
            get { return MethodName; }
        }
    }
}