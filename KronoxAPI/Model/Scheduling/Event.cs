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
    private readonly string _title;
    private readonly string _courseId;
    private readonly List<Teacher> _teachers;
    private readonly DateTime _timeStart;
    private readonly DateTime _timeEnd;
    private readonly List<Location> _locations;
    private readonly bool _isSpecial;

    public string Title => _title;

    public string CourseId => _courseId;

    public DateTime TimeStart => _timeStart;

    public DateTime TimeEnd => _timeEnd;

    public List<Location> Locations => _locations;

    public List<Teacher> Teachers => _teachers;

    public string Id => _id;

    public bool IsSpecial => _isSpecial;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="course"></param>
    /// <param name="teachers"></param>
    /// <param name="timeStart"></param>
    /// <param name="timeEnd"></param>
    /// <param name="locations"></param>
    public Event(string title, string courseId, List<Teacher> teachers, DateTime timeStart, DateTime timeEnd, List<Location> locations, bool isSpecial)
    {
        _id = Guid.NewGuid().ToString();
        _title = title;
        _courseId = courseId;
        _timeStart = timeStart;
        _timeEnd = timeEnd;
        _locations = locations;
        _teachers = teachers;
        _isSpecial = isSpecial;
    }

    public override string? ToString()
    {
        return $"Id: {_id}\nCourse: {_courseId}\nTitle: {_title}\nStarts: {_timeStart:yyyy-MM-dd HH:mm}" +
            $"\nEnds: {_timeEnd:yyyy-MM-dd HH:mm}\nTeachers: [{String.Join(", ", _teachers)}]\nLocations: [{String.Join(", ", _locations)}]";
    }
}
