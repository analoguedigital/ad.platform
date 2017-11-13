using System;

namespace AppHelper
{
    public static class ObjectExtensions
    {
        public static void Perform<T>(this T item, Action<T> action) where T : class
        {
            if (item != null)
                action(item);
        }

        public static K Get<T, K>(this T item, Func<T, K> selector)
        {
            if (object.ReferenceEquals(item, null))
                return default(K);
            else return selector(item);
        }


        public static K? Get<T, K>(this T item, Func<T, K?> selector) where K : struct
        {
            if (item == null) return null;

            return selector(item);
        }

        public static Guid? Get<T>(this T item, Func<T, Guid> selector) where T : class
        {
            if (item == null) return null;

            return selector(item);
        }

        public static int? Get<T>(this T item, Func<T, int> selector) where T : class
        {
            if (item == null) return null;

            return selector(item);
        }


        public static double? Get<T>(this T item, Func<T, double> selector) where T : class
        {
            if (item == null) return null;
            return selector(item);
        }


        public static decimal? Get<T>(this T item, Func<T, decimal> selector) where T : class
        {
            if (item == null) return null;
            return selector(item);
        }


        public static bool? Get<T>(this T item, Func<T, bool> selector) where T : class
        {
            if (item == null) return null;
            return selector(item);
        }


        public static string Get(this DateTime? item, Func<DateTime?, string> selector)
        {
            if (item == null) return null;
            return selector(item);
        }

        public static byte? Get<T>(this T item, Func<T, byte> selector) where T : class
        {
            if (item == null) return null;
            return selector(item);
        }


        public static DateTime? Get<T>(this T item, Func<T, DateTime> selector) where T : class
        {
            if (item == null) return null;
            return selector(item);
        }


        public static DateTime? Get<T>(this T item, Func<T, DateTime?> selector) where T : class
        {
            if (item == null) return null;
            return selector(item);
        }


        public static T Get<T>(this DateTime? item, Func<DateTime?, T> selector) where T : struct
        {
            if (item == null) return default(T);
            return selector(item);
        }
    }
}
