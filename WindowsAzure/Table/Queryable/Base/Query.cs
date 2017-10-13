using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WindowsAzure.Table.Queryable.Base
{
    /// <summary>
    ///     Provides a base implementation of the queryable.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Query<T> : IOrderedQueryable<T>
    {
        private readonly Expression _expression;

        protected Query()
        {
            _expression = Expression.Constant(this);
        }

        /// <summary>
        ///     Creates a query.
        /// </summary>
        /// <param name="provider">Query provider.</param>
        /// <param name="expression">Expression.</param>
        internal Query(IQueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (!typeof (IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException(nameof(expression));
            }

            Provider = provider;

            _expression = expression;
        }

        Expression IQueryable.Expression => _expression;

        Type IQueryable.ElementType => typeof (T);

        public IQueryProvider Provider { get; protected set; }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) Provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Provider.Execute(_expression)).GetEnumerator();
        }
    }
}