using System;
using System.Collections.Generic;
#if WINDOWSAZURE
using Microsoft.WindowsAzure.Storage.Table;
#else
using Microsoft.Azure.Cosmos.Table;
#endif

namespace WindowsAzure.Table.Wrappers
{
    /// <summary>
    ///     Wrapper around <see cref="TableQuery" />.
    /// </summary>
    internal sealed class TableQueryWrapper : ITableQuery
    {
        private readonly TableQuery _tableQuery;

        /// <summary>
        ///     Constructor.
        /// </summary>
        /// <param name="tableQuery">Table query.</param>
        public TableQueryWrapper(TableQuery tableQuery)
        {
            if (tableQuery == null)
            {
                throw new ArgumentNullException(nameof(tableQuery));
            }

            _tableQuery = tableQuery;
        }

        /// <summary>
        ///     Gets or sets the number of entities the table query will return.
        /// </summary>
        /// <value>
        ///     The maximum number of entities for the table query to return.
        /// </value>
        public int? TakeCount
        {
            get { return _tableQuery.TakeCount; }
            set { _tableQuery.TakeCount = value; }
        }

        /// <summary>
        ///     Gets or sets the filter expression to use in the table query.
        /// </summary>
        /// <value>
        ///     A string containing the filter expression to use in the query.
        /// </value>
        public string FilterString
        {
            get { return _tableQuery.FilterString; }
            set { _tableQuery.FilterString = value; }
        }

        /// <summary>
        ///     Gets or sets the property names of the table entity properties to return when the table query is executed.
        /// </summary>
        /// <value>
        ///     A list of strings containing the property names of the table entity properties to return when the query is executed.
        /// </value>
        public IList<string> SelectColumns
        {
            get { return _tableQuery.SelectColumns; }
            set { _tableQuery.SelectColumns = value; }
        }
    }
}