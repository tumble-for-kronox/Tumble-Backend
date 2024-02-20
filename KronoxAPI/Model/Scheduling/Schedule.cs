using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

/// <summary>
/// Model for storing schedule data fetched from Kronox's database.
/// </summary>
public class Schedule
{
    public Schedule(string[] ids, List<Day> days)
    {
        Ids = ids;
        Days = days;
    }

    public string[] Ids { get; }

    public List<Day> Days { get; set; }
}
