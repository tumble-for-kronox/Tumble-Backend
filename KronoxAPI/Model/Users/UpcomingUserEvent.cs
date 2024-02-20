namespace KronoxAPI.Model.Users;

public class UpcomingUserEvent : UserEvent
{
    public UpcomingUserEvent(string title, string type, DateTime firstSignupDate, DateTime eventStart, DateTime eventEnd) : base(title, type, eventStart, eventEnd)
    {
        FirstSignupDate = firstSignupDate;
    }

    public DateTime FirstSignupDate { get; }

    /// <summary>
    /// For use as default or in case an event is not found/can't be parsed.
    /// </summary>
    /// <returns><see cref="UpcomingUserEvent"/> wiht all values set as "N/A".</returns>
    public static UpcomingUserEvent NotAvailable => new("N/A", "N/A", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
}
