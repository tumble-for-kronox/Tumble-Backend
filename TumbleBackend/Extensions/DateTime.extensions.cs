namespace TumbleBackend.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime date)
        {
            return date.Subtract(new TimeSpan((int)date.DayOfWeek - 1, 0, 0, 0));
        }
    }
}
