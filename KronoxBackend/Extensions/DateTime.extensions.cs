namespace KronoxBackend.Extensions;

public static class DateTimeExtensions
{
    public static DateTime FirstDayOfWeek(this DateTime date)
    {
        while (date.DayOfWeek != DayOfWeek.Monday)
            date = date.AddDays(-1);
        return date;
    }
}
