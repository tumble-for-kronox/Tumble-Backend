using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

/// <summary>
/// Model for storing data regarding Events (classes) from Kronox's database.
/// </summary>
public class Event
{
    private readonly string _id;
    private readonly string _scheduleId;
    private readonly string _title;
    private readonly Course _course;
    private readonly List<Teacher> _teachers;
    private readonly DateTime _timeStart;
    private readonly DateTime _timeEnd;
    private readonly List<Location> _locations;
    private readonly DateTime _lastModified;
    private readonly bool _isSpecial;

    public string Title => _title;

    public Course Course => _course;

    public DateTime TimeStart => _timeStart;

    public DateTime TimeEnd => _timeEnd;

    public List<Location> Locations => _locations;

    public List<Teacher> Teachers => _teachers;

    public string Id => _id;

    public string ScheduleId => _scheduleId;

    public bool IsSpecial => _isSpecial;

    public DateTime LastModified => _lastModified;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="scheduleId"></param>
    /// <param name="title"></param>
    /// <param name="course"></param>
    /// <param name="teachers"></param>
    /// <param name="timeStart"></param>
    /// <param name="timeEnd"></param>
    /// <param name="locations"></param>
    /// <param name="isSpecial"></param>
    /// <param name="lastModified"></param>
    public Event(string id, string scheduleId, string title, Course course, List<Teacher> teachers, DateTime timeStart, DateTime timeEnd, List<Location> locations, bool isSpecial, DateTime lastModified)
    {
        _id = id;
        _scheduleId = scheduleId;
        _title = title;
        _course = course;
        _timeStart = timeStart;
        _timeEnd = timeEnd;
        _locations = locations;
        _teachers = teachers;
        _lastModified = lastModified;
        _isSpecial = isSpecial;
    }

    public override string? ToString()
    {
        return $"Id: {_id}\nScheduleId: {_scheduleId}\nCourse: {_course}\nTitle: {_title}\nStarts: {_timeStart:yyyy-MM-dd HH:mm}" +
            $"\nEnds: {_timeEnd:yyyy-MM-dd HH:mm}\nTeachers: [{String.Join(", ", _teachers)}]\nLocations: [{String.Join(", ", _locations)}]";
    }
}
