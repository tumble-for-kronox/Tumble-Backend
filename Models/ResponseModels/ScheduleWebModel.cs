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

namespace WebAPIModels.ResponseModels;

public class ScheduleWebModel
{
    private string _id;
    private DateTime _cachedAt;
    private List<DayWebModel> _days;

    public string Id { get => _id; set => _id = value; }

    public DateTime CachedAt { get => _cachedAt; private set => _cachedAt = value; }

    public List<DayWebModel> Days { get => _days; set => _days = value; }


    public void UpdateCachedAt()
    {
        CachedAt = DateTime.Now;
    }

    public ScheduleWebModel(string id, DateTime cachedAt, List<DayWebModel> days)
    {
        _id = id;
        _cachedAt = cachedAt;
        _days = days;
    }
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

}
