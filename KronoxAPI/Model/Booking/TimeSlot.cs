using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class TimeSlot
{
    private readonly DateTime _from;
    private readonly DateTime _to;
    private readonly TimeSpan _duration;

    public TimeSlot(DateTime from, DateTime to)
    {
        _from = from;
        _to = to;
        _duration = to.Subtract(from);
    }

    public DateTime From => _from;

    public DateTime To => _to;

    public TimeSpan Duration => _duration;

    public DateTime GetConfirmationOpens => From.Subtract(TimeSpan.FromMinutes(30));

    public DateTime GetConfirmationCloses => From.Add(TimeSpan.FromMinutes(15));

    public override string? ToString()
    {
        return $"{From:HH:mm}-{To:HH:mm}";
    }

    public override int GetHashCode()
    {
        return ToString()!.GetHashCode();
    }

    public override bool Equals(Object? obj)
    {
        if (obj == null || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TimeSlot t = (TimeSlot)obj;
            return From == t.From && To == t.To;
        }
    }
}
