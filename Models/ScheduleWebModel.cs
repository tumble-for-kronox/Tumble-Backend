using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KronoxAPI.Model.Scheduling;

namespace WebAPIModels;

public class ScheduleWebModel
{
    private readonly string _id;
    private string _cachedAt;
    private readonly List<Day> _days;

    public string Id => _id;

    public string CachedAt { get => _cachedAt; private set => _cachedAt = value; }

    public List<Day> Days => _days;

    public void UpdateCachedAt()
    {
        CachedAt = DateTime.Now.ToString("o");
    }

    public ScheduleWebModel(string id, string cachedAt, List<Day> days)
    {
        _id = id;
        _cachedAt = cachedAt;
        _days = days;
    }
}
