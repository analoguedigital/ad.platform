using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AppHelper
{
    public static class FormCollectionExtensions
    {
        public static ICollection<KeyValuePair<string, string>> ToKeyValuePairs(this FormCollection collection)
        {
            return collection.AllKeys.Distinct().Select(k => new KeyValuePair<string, string>(k, collection[k].ToString())).ToList();
        }
    }
}
