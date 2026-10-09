using System.Globalization;

namespace StrategyMate.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ToPersianDate(this DateTime date)
        {
            var pc = new PersianCalendar();
            return $"{pc.GetYear(date)}/{pc.GetMonth(date):D2}/{pc.GetDayOfMonth(date):D2}";
        }
    }
}
