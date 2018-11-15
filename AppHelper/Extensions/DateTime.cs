using System;
using System.Globalization;

namespace AppHelper
{
    public static class DateTimeExtensions
    {
        public static string ToString(this DateTime date, string format, string calendar)
        {
            if (calendar.Equals("persian", StringComparison.InvariantCultureIgnoreCase))
            {
                return DateToPersian(date, format);
            }

            return date.ToString(format);
        }

        private static string DateToPersian(DateTime date, string format)
        {
            var abbreviatedDayNames = new string[] { "ي", "د", "س", "چ", "پ", "ج", "ش" };
            var dayNames = new string[] { "يكشنبه", "دوشنبه", "سه شنبه", "چهار شنبه", "پنجشنبه", "جمعه", "شنبه" };
            var monthNames = new string[] { "فروردين", "ارديبهشت", "خرداد", "تير", "مرداد", "شهريور", "مهر", "آبان", "آذر", "دي", "بهمن", "اسفند", "" };

            var pc = new PersianCalendar();

            if (format == "d")
                return string.Format("{2}-{1}-{0}", pc.GetDayOfMonth(date), pc.GetMonth(date), pc.GetYear(date));

            if (format == "D")
                return string.Format("{0}, {1} {2} {3}", dayNames[(int)pc.GetDayOfWeek(date)], pc.GetDayOfMonth(date), monthNames[pc.GetMonth(date) - 1], pc.GetYear(date));

            return string.Format("{0} {1} {2}", pc.GetDayOfMonth(date), monthNames[pc.GetMonth(date) - 1], pc.GetYear(date));
        }
    }
}
