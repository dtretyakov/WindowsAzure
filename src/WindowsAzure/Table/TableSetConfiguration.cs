using WindowsAzure.Table.EntityConverters;
using WindowsAzure.Table.Queryable.Expressions;

namespace WindowsAzure.Table
{
    /// <summary>
    ///     Manages settings of the TableSet context.
    /// </summary>
    public sealed class TableSetConfiguration<T> where T : new()
    {
        /// <summary>
        ///     Gets or sets a table entity converter.
        /// </summary>
        public ITableEntityConverter<T> EntityConverter { get; set; }

        /// <summary>
        ///     Gets or sets an expression query translator.
        /// </summary>
        public IQueryTranslator QueryTranslator { get; set; }
    }
}