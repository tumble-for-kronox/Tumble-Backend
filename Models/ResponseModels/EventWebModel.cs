using KronoxAPI.Model.Scheduling;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.ResponseModels;

public class EventWebModel
{
    private readonly string _id;
    private readonly string[] _scheduleIds;
    private readonly string _title;
    private readonly CourseWebModel _course;
    private readonly List<Teacher> _teachers;
    private readonly DateTime _from;
    private readonly DateTime _to;
    private readonly List<Location> _locations;
    private readonly DateTime _lastModified;
    private readonly bool _isSpecial;

    public string Title => _title;

    public CourseWebModel Course => _course;

    public DateTime From => _from;

    public DateTime To => _to;

    public List<Location> Locations => _locations;

    public List<Teacher> Teachers => _teachers;

    public string Id => _id;

    public string[] ScheduleIds => _scheduleIds;

    public bool IsSpecial => _isSpecial;

    public DateTime LastModified => _lastModified;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="course"></param>
    /// <param name="teachers"></param>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="locations"></param>
    public EventWebModel(string id, string[] scheduleIds, string title, CourseWebModel course, List<Teacher> teachers, DateTime from, DateTime to, List<Location> locations, bool isSpecial, DateTime lastModified)
    {
        _id = id;
        _scheduleIds = scheduleIds;
        _title = title;
        _course = course;
        _from = from;
        _to = to;
        _locations = locations;
        _teachers = teachers;
        _lastModified = lastModified;
        _isSpecial = isSpecial;
    }
}
