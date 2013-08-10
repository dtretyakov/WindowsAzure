using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;
using WindowsAzure.Table.Wrappers;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     TableQuery translation result.
    /// </summary>
    internal sealed class TranslationResult : ITranslationResult
    {
        private readonly List<LambdaExpression> _expressions = new List<LambdaExpression>();
        private readonly TableQueryWrapper _tableQuery = new TableQueryWrapper(new TableQuery());
        private int _filtersCount;

        /// <summary>
        ///     Gets a TableQuery.
        /// </summary>
        public ITableQuery TableQuery
        {
            get { return _tableQuery; }
        }

        /// <summary>
        ///     Gets a post processing handler.
        /// </summary>
        public Delegate PostProcessing
        {
            get { return MergeLambdasAndCompile(_expressions); }
        }

        /// <summary>
        ///     Adds a filter expression.
        /// </summary>
        /// <param name="filter">Filter expression.</param>
        public void AddFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                throw new ArgumentNullException("filter");
            }

            if (_filtersCount == 0)
            {
                _tableQuery.FilterString = filter;
            }
            else
            {
                // Combine filters
                var stringBuilder = new StringBuilder(_tableQuery.FilterString.Length + filter.Length);

                if (_filtersCount > 0)
                {
                    if (_tableQuery.FilterString.Count(p => p == ' ') > 2)
                    {
                        stringBuilder.AppendFormat("({0})", _tableQuery.FilterString);
                    }
                    else
                    {
                        stringBuilder.Append(_tableQuery.FilterString);
                    }
                }

                stringBuilder.Append(" and ");

                if (filter.Count(p => p == ' ') > 2)
                {
                    stringBuilder.AppendFormat("({0})", filter);
                }
                else
                {
                    stringBuilder.Append(filter);
                }

                _tableQuery.FilterString = stringBuilder.ToString();
            }

            _filtersCount++;
        }

        /// <summary>
        ///     Adds a required count of query elements.
        /// </summary>
        /// <param name="top">Count of elements.</param>
        public void AddTop(int top)
        {
            if (top <= 0)
            {
                throw new ArgumentOutOfRangeException("top");
            }

            _tableQuery.TakeCount = top;
        }

        /// <summary>
        ///     Adds a required column name.
        /// </summary>
        /// <param name="column">Column name.</param>
        public void AddColumn(string column)
        {
            if (string.IsNullOrEmpty(column))
            {
                throw new ArgumentNullException("column");
            }

            if (_tableQuery.SelectColumns == null)
            {
                _tableQuery.SelectColumns = new List<string>();
            }

            _tableQuery.SelectColumns.Add(column);
        }

        /// <summary>
        ///     Adds a post processing expression.
        /// </summary>
        /// <param name="lambda">Expression.</param>
        public void AddPostProcesing(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException("lambda");
            }

            _expressions.Add(lambda);
        }

        /// <summary>
        ///     Merges lambda expressions.
        /// </summary>
        /// <param name="expressions">List of lambda expressions.</param>
        /// <returns>Delegate.</returns>
        private static Delegate MergeLambdasAndCompile(IList<LambdaExpression> expressions)
        {
            if (expressions.Count == 0)
            {
                return null;
            }

            LambdaExpression lambda = expressions[0];

            for (int i = 1; i < expressions.Count; i++)
            {
                InvocationExpression invoked = Expression.Invoke(expressions[i], lambda.Body);
                lambda = Expression.Lambda(invoked, lambda.Parameters);
            }

            return lambda.Compile();
        }
    }
}