using KronoxAPI.Model.Scheduling;
using WebAPIModels.Extensions;
using System.Globalization;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.Extensions;

public static class ListExtensions
{
    public static List<DayWebModel> PadScheduleDays(this List<DayWebModel> days, DateTime startDate)
    {
        Dictionary<string, DayWebModel> groupedDays = days.ToDictionary(day => day.Date, day => day);

        DateTime endDate = startDate.AddMonths(6);
        List<DayWebModel> paddedDays = new();

        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            if (groupedDays.ContainsKey(date.ToString("d/M", CultureInfo.InvariantCulture)))
            {
                paddedDays.Add(groupedDays[date.ToString("d/M", CultureInfo.InvariantCulture)]);
                continue;
            }

            paddedDays.Add(new Day(date.DayOfWeek.ToString(), date, new()).ToWebModel(new()));
        }

        return paddedDays;
    }
}
