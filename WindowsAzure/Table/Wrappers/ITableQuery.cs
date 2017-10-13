using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Wrappers
{
    /// <summary>
    ///     <see cref="TableQuery" /> public interface.
    /// </summary>
    internal interface ITableQuery
    {
        /// <summary>
        ///     Gets or sets the number of entities the table query will return.
        /// </summary>
        /// <value>
        ///     The maximum number of entities for the table query to return.
        /// </value>
        int? TakeCount { get; set; }

        /// <summary>
        ///     Gets or sets the filter expression to use in the table query.
        /// </summary>
        /// <value>
        ///     A string containing the filter expression to use in the query.
        /// </value>
        string FilterString { get; set; }

        /// <summary>
        ///     Gets or sets the property names of the table entity properties to return when the table query is executed.
        /// </summary>
        /// <value>
        ///     A list of strings containing the property names of the table entity properties to return when the query is executed.
        /// </value>
        IList<string> SelectColumns { get; set; }
    }
}