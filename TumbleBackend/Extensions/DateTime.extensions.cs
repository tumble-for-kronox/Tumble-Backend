namespace TumbleBackend.Extensions;

public static class DateTimeExtensions
{
    public static DateTime FirstDayOfWeek(this DateTime date)
    {
        return date.AddDays(-(int)date.DayOfWeek + (int)DayOfWeek.Monday);
    }
}
