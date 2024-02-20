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
    public string Name { get; }

    public DateTime Date { get; }

    public int WeekNumber { get; }

    public List<Event> Events { get; }

    public string Id { get; }
    
    public Day(string name, DateTime date, List<Event> events)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Date = date;
        Events = events;

        WeekNumber = Utilities.DateUtils.GetIso8601WeekOfYear(date);
    }
}
