using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace WindowsAzure.Table.Queryable.Base
{
    public class Query<T> : IOrderedQueryable<T>
    {
        private readonly Expression _expression;
        private readonly IQueryProvider _provider;

        public Query(IQueryProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            _provider = provider;
            _expression = Expression.Constant(this);
        }

        public Query(QueryProviderBase provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }

            if (!typeof (IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }

            _provider = provider;
            _expression = expression;
        }

        Expression IQueryable.Expression
        {
            get { return _expression; }
        }

        Type IQueryable.ElementType
        {
            get { return typeof (T); }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
        }
    }
}