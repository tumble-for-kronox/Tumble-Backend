namespace TumbleBackend.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime date)
        {
            return date.Subtract(new TimeSpan((int)date.DayOfWeek, 0, 0, 0));
        }
    }
}
