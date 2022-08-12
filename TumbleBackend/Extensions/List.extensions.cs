using KronoxAPI.Model.Scheduling;
using WebAPIModels.Extensions;
using System.Globalization;
using WebAPIModels.ResponseModels;
using KronoxAPI.Model.Schools;

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

    public static List<Programme> FilterWorkingProgrammeLinks(this List<Programme> programmes, School school, string? sessionToken)
    {
        bool[] programmeAvailability = Task.WhenAll(programmes.Select(programme => programme.ScheduleAvailable(school, sessionToken))).Result;

        return programmes.Where((programme, i) => programmeAvailability[i]).ToList();
    }
}
