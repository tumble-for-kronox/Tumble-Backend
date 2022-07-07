using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

/// <summary>
/// Model to group together Events from Kronox based on which days they are in.
/// </summary>
public class Day
{
    private readonly string _id;
    private readonly string _name;
    private readonly int _weekNumber;
    private readonly DateOnly _date;
    private readonly List<Event> _events;

    public string Name => _name;

    public DateOnly Date => _date;

    public int WeekNumber => _weekNumber;

    public List<Event> Events => _events;

    public string Id => _id;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="date"></param>
    /// <param name="events"></param>
    public Day(string name, DateOnly date, List<Event> events)
    {
        _id = Guid.NewGuid().ToString();
        _name = name;
        _date = date;
        _events = events;

        _weekNumber = Utilities.DateUtils.GetIso8601WeekOfYear(date.ToDateTime(TimeOnly.MinValue));
    }
}
