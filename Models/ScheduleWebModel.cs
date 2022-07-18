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
    private string _cachedAt;
    private readonly List<DayWebModel> _days;

    public string Id => _id;

    public string CachedAt { get => _cachedAt; private set => _cachedAt = value; }

    public List<DayWebModel> Days => _days;


    public void UpdateCachedAt()
    {
        CachedAt = DateTime.Now.ToString("o");
    }

    public ScheduleWebModel(string id, string cachedAt, List<DayWebModel> days)
    {
        _id = id;
        _cachedAt = cachedAt;
        _days = days;
    }
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

}
