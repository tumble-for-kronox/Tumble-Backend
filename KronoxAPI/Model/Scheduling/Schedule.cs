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
    private readonly string _id;
    private readonly List<Day> _days;
    private readonly Dictionary<string, Course> _courses;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="days"></param>
    /// <param name="courses"></param>
    public Schedule(string id, List<Day> days, Dictionary<string, Course> courses)
    {
        _id = id;
        _days = days;
        _courses = courses;
    }

    public string Id => _id;

    public List<Day> Days => _days;

    public Dictionary<string, Course> Courses => _courses;
}
