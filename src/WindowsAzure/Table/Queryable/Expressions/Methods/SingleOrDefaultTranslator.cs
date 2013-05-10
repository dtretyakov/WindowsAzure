using System.Collections.Generic;

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

        public override string Name
        {
            get { return MethodName; }
        }
    }
}