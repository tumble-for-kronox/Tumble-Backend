using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebAPIModels.ResponseModels;

public class MultiScheduleWebModel
{
    private string[] _ids;
    private DateTime _cachedAt;
    private List<DayWebModel> _days;

    public string[] Ids { get => _ids; set => _ids = value; }

    public DateTime CachedAt { get => _cachedAt; private set => _cachedAt = value; }

    public List<DayWebModel> Days { get => _days; set => _days = value; }

    public MultiScheduleWebModel Combine(MultiScheduleWebModel schedule)
    {
        List<DayWebModel> newDays = new();

        for (int i = 0; i < _days.Count; i++)
        {
            DayWebModel currDay = _days[i];
            DayWebModel combineDay = schedule.Days[i];

            List<EventWebModel> combinedEventList = currDay.Events.Concat(combineDay.Events).ToList();

            List<EventWebModel> uniqueEventList = combinedEventList.GroupBy(x => x.Id).Select(x => x.First()).ToList();

            newDays.Add(new(currDay.Name, currDay.Date, currDay.IsoString, currDay.WeekNumber, uniqueEventList));
        }

        return new(Ids, CachedAt, newDays);
    }

    public List<EventWebModel> GetEvents()
    {
        return _days.SelectMany(day => day.Events).ToList();
    }

    public void UpdateCachedAt()
    {
        CachedAt = DateTime.Now;
    }

    public MultiScheduleWebModel(string[] ids, DateTime cachedAt, List<DayWebModel> days)
    {
        _ids = ids;
        _cachedAt = cachedAt;
        _days = days;
    }
    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
