using System.Globalization;

namespace MakeenBot.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToPersianDateTextify(this DateTime dateTime)
        {
            var pc = new PersianCalendar();
            int year = pc.GetYear(dateTime);
            int month = pc.GetMonth(dateTime);
            int day = pc.GetDayOfMonth(dateTime);

            return $"{year:0000}/{month:00}/{day:00}";
        }
    }
}
