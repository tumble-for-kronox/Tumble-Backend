using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class TimeSlot
{
    public TimeSlot(DateTime from, DateTime to, int? id = null)
    {
        Id = id;
        From = from;
        To = to;
        Duration = to.Subtract(from);
    }

    public int? Id { get; }

    public DateTime From { get; }

    public DateTime To { get; }

    public TimeSpan Duration { get; }

    public override string? ToString()
    {
        return $"{From:HH:mm}-{To:HH:mm}";
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        var t = (TimeSlot)obj;
        return Id == t.Id;
    }
}
