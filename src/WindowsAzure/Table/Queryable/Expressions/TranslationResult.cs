using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.WindowsAzure.Storage.Table;

namespace WindowsAzure.Table.Queryable.Expressions
{
    /// <summary>
    ///     TableQuery translation result.
    /// </summary>
    public sealed class TranslationResult : ITranslationResult
    {
        private readonly List<LambdaExpression> _expressions = new List<LambdaExpression>();
        private Delegate _postProcessing;

        /// <summary>
        ///     Constructor.
        /// </summary>
        public TranslationResult()
        {
            TableQuery = new TableQuery();
        }

        /// <summary>
        ///     Gets a TableQuery.
        /// </summary>
        public TableQuery TableQuery { get; private set; }

        /// <summary>
        ///     Gets a post processing handler.
        /// </summary>
        public Delegate PostProcessing
        {
            get
            {
                return _postProcessing ??
                       (_postProcessing = _expressions.Count == 0
                                              ? null
                                              : MergeLambdasAndCompile(_expressions));
            }
        }

        public void AddFilter(string filter)
        {
            TableQuery.FilterString = filter;
        }

        public void AddTop(int top)
        {
            TableQuery.TakeCount = top;
        }

        public void AddColumn(string column)
        {
            if (TableQuery.SelectColumns == null)
            {
                TableQuery.SelectColumns = new List<string>();
            }

            TableQuery.SelectColumns.Add(column);
        }

        public void AddPostProcesing(LambdaExpression lambda)
        {
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