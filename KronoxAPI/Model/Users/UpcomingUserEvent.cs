using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Users;

public class UpcomingUserEvent : UserEvent
{
    private readonly DateTime _firstSignupDate;

    public UpcomingUserEvent(string title, string type, DateTime firstSignupDate, DateTime eventStart, DateTime eventEnd) : base(title, type, eventStart, eventEnd)
    {
        _firstSignupDate = firstSignupDate;
    }

    public DateTime FirstSignupDate => _firstSignupDate;

    /// <summary>
    /// For use as default or in case an event is not found/can't be parsed.
    /// </summary>
    /// <returns><see cref="UpcomingUserEvent"/> wiht all values set as "N/A".</returns>
    public static UpcomingUserEvent NotAvailable => new("N/A", "N/A", DateTime.MinValue, DateTime.MinValue, DateTime.MinValue);
}
