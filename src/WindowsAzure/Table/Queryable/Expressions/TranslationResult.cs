using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     TableQuery translation result.
    /// </summary>
    internal sealed class TranslationResult : ITranslationResult
    {
        private readonly List<LambdaExpression> _expressions = new List<LambdaExpression>();
        private readonly TableQuery _tableQuery = new TableQuery();
        private Delegate _postProcessing;
        private int filtersCount;

        /// <summary>
        ///     Gets a TableQuery.
        /// </summary>
        public TableQuery TableQuery
        {
            get { return _tableQuery; }
        }

        /// <summary>
        ///     Gets a post processing handler.
        /// </summary>
        public Delegate PostProcessing
        {
            get
            {
                if (_postProcessing != null)
                {
                    return _postProcessing;
                }

                if (_expressions.Count == 0)
                {
                    return null;
                }

                _postProcessing = MergeLambdasAndCompile(_expressions);

                return _postProcessing;
            }
        }

        public void AddFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                throw new ArgumentNullException("filter");
            }

            if (filtersCount == 0)
            {
                _tableQuery.FilterString = filter;
            }
            else
            {
                // Combine filters
                var stringBuilder = new StringBuilder(_tableQuery.FilterString.Length + filter.Length);

                if (filtersCount == 1)
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

            filtersCount++;
        }

        public void AddTop(int top)
        {
            if (top <= 0)
            {
                throw new ArgumentOutOfRangeException("top");
            }

            _tableQuery.TakeCount = top;
        }

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

        public void AddPostProcesing(LambdaExpression lambda)
        {
            if (lambda == null)
            {
                throw new ArgumentNullException("lambda");
            }

            _expressions.Add(lambda);
        }

        private static Delegate MergeLambdasAndCompile(IList<LambdaExpression> transformations)
        {
            LambdaExpression lambda = MergeLambdas(transformations);
            if (lambda == null)
            {
                return null;
            }

            return lambda.Compile();
        }

        private static LambdaExpression MergeLambdas(IList<LambdaExpression> transformations)
        {
            if (transformations == null || transformations.Count == 0)
            {
                return null;
            }

            LambdaExpression lambda = transformations[0];

            for (int i = 1; i < transformations.Count; i++)
            {
                InvocationExpression invoked = Expression.Invoke(transformations[i], lambda.Body);
                lambda = Expression.Lambda(invoked, lambda.Parameters);
            }

            return lambda;
        }
    }
}