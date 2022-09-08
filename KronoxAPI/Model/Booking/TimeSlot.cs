using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class TimeSlot
{
    private readonly int? _id;
    private readonly DateTime _from;
    private readonly DateTime _to;
    private readonly TimeSpan _duration;

    public TimeSlot(DateTime from, DateTime to, int? id = null)
    {
        _id = id;
        _from = from;
        _to = to;
        _duration = to.Subtract(from);
    }

    public int? Id => _id;

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
        return Id.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || !GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            TimeSlot t = (TimeSlot)obj;
            return Id == t.Id;
        }
    }
}
