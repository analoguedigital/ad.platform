using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {
        public static IOrderedEnumerable<T> OrderBy<T, TKey>(this IEnumerable<T> list, Func<T, TKey> selector, bool ascending = true)
        {
            if (ascending)
                return list.OrderBy(selector);
            return list.OrderByDescending(selector);
        }

        public static IEnumerable<SelectListItem> ToSelectList<T>(this IEnumerable<T> list, string dataValueField, string dataTextField, object selectedValue = null)
        {
            return new SelectList(list, dataValueField, dataTextField, selectedValue);
        }

        public static string ToString(this IEnumerable<object> list, string seperator)
        {
            return String.Join(seperator, list.Select(o => o.ToString()));
        }

        public static IEnumerable<T> Except<T, TKey>(this IEnumerable<T> items, IEnumerable<T> other,
                                                                            Func<T, TKey> getKey)
        {
            return from item in items
                   join otherItem in other on getKey(item)
                   equals getKey(otherItem) into tempItems
                   from temp in tempItems.DefaultIfEmpty()
                   where ReferenceEquals(null, temp) || temp.Equals(default(T))
                   select item;

        }
    }
}
