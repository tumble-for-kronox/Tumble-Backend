using KronoxAPI.Model.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.ResponseModels;

public class EventWebModel
{
    private readonly string _id;
    private readonly string _title;
    private readonly CourseWebModel _course;
    private readonly List<Teacher> _teachers;
    private readonly DateTime _timeStart;
    private readonly DateTime _timeEnd;
    private readonly List<Location> _locations;
    private readonly DateTime _lastModified;
    private readonly bool _isSpecial;

    public string Title => _title;

    public CourseWebModel Course => _course;

    public DateTime TimeStart => _timeStart;

    public DateTime TimeEnd => _timeEnd;

    public List<Location> Locations => _locations;

    public List<Teacher> Teachers => _teachers;

    public string Id => _id;

    public bool IsSpecial => _isSpecial;

    public DateTime LastModified => _lastModified;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="course"></param>
    /// <param name="teachers"></param>
    /// <param name="timeStart"></param>
    /// <param name="timeEnd"></param>
    /// <param name="locations"></param>
    public EventWebModel(string title, CourseWebModel course, List<Teacher> teachers, DateTime timeStart, DateTime timeEnd, List<Location> locations, bool isSpecial, DateTime lastModified)
    {
        _id = Guid.NewGuid().ToString();
        _title = title;
        _course = course;
        _timeStart = timeStart;
        _timeEnd = timeEnd;
        _locations = locations;
        _teachers = teachers;
        _lastModified = lastModified;
        _isSpecial = isSpecial;
    }
}
