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

        protected Query()
        {
            _expression = Expression.Constant(this);
        }

        public Query(IQueryProvider provider, Expression expression)
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

            Provider = provider;

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