using System;
using System.Linq;

namespace AppHelper
{
    public static class IQuaryableExtensions
    {
        public static IOrderedQueryable<T> OrderBy<T, TKey>(this IQueryable<T> list, System.Linq.Expressions.Expression<Func<T, TKey>> selector, bool ascending)
        {
            if (ascending)
                return list.OrderBy(selector);
            return (IOrderedQueryable<T>)list.OrderByDescending(selector);
        }
    }
}
