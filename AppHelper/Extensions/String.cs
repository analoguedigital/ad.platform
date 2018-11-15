using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace System
{
    public static class StringExtensions
    {
        public static string SplitCamelCase(this string s)
        {
            Regex r = new Regex(
               @"  (?<=[A-Z])(?=[A-Z][a-z])    # UC before me, UC lc after me
                |  (?<=[^A-Z])(?=[A-Z])        # Not UC before me, UC after me
                |  (?<=[A-Za-z])(?=[^A-Za-z])  # Letter before me, non letter after me
                ",
               RegexOptions.IgnorePatternWhitespace
            );

            return string.Join(" ", r.Split(s));
        }

        public static bool HasValue(this string str)
        {
            if (str == null)
                return false;

            if (str == string.Empty)
                return false;

            return true;
        }

        public static string OnlyIf(this string str, bool condition)
        {
            if (condition) return str;
            return string.Empty;
        }

        public static string FormatWith(this string str, params string[] p)
        {
            return string.Format(str, p);
        }

        public static Guid ToGuid(this string str)
        {
            return Guid.Parse(str);
        }

        public static bool IsEmpty(this string str)
        {
            return String.IsNullOrEmpty(str);
        }
        public static bool Contains(this string str, string value, bool caseSensitive)
        {
            if (caseSensitive)
                return str.Contains(value);
            return str.ToLower().Contains(value.ToLower());
        }

        public static string ToString(this IEnumerable<string> list, string sep)
        {
            return String.Join(sep, list);
        }

        public static bool ContainsAny(this IEnumerable<string> list, params string[] items)
        {
            return items.Intersect(list).Any();
        }
    }
}
