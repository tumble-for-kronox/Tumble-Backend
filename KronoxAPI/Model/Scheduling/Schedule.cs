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
    private readonly string[] _ids;
    private List<Day> _days;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="days"></param>
    /// <param name="courses"></param>
    public Schedule(string[] ids, List<Day> days)
    {
        _ids = ids;
        _days = days;
    }

    public string[] Ids => _ids;

    public List<Day> Days { get => _days; set => _days = value; }
}
