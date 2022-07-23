using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KronoxAPI.Model.Scheduling;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIModels;

public class ScheduleWebModel
{
    private readonly string _id;
    private DateTime _cachedAt;
    private readonly List<DayWebModel> _days;
    private readonly Dictionary<string, CourseWebModel> _courses;

    public string Id => _id;

    public DateTime CachedAt { get => _cachedAt; private set => _cachedAt = value; }

    public List<DayWebModel> Days => _days;

    public Dictionary<string, CourseWebModel> Courses => _courses;


    public void UpdateCachedAt()
    {
        CachedAt = DateTime.Now;
    }

    public ScheduleWebModel(string id, DateTime cachedAt, List<DayWebModel> days, Dictionary<string, CourseWebModel> courses)
    {
        _id = id;
        _cachedAt = cachedAt;
        _days = days;
        _courses = courses;
    }
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

}
