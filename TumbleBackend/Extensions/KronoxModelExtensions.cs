using WebAPIModels;
using System.Linq;
using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Users;



namespace TumbleBackend.Extensions;

static class KronoxModelExtensions
{
    /// <summary>
    /// Create a Web Model object from a given <see cref="Schedule"/> object with the current time as the CachedAt value.
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns>A <see cref="ScheduleWebModel"/> with the current time as its CachedAt value.</returns>
    public static ScheduleWebModel ToWebModel(this Schedule schedule)
    {
        return new ScheduleWebModel(schedule.Id, DateTime.Now.ToString("o"), schedule.Days);
    }

    /// <summary>
    /// Create a Web Model object from a given <see cref="Schedule"/> object with the given <paramref name="cachedAt"/> datetime as the CachedAt value.
    /// </summary>
    /// <param name="schedule"></param>
    /// <param name="cachedAt"></param>
    /// <returns>A <see cref="ScheduleWebModel"/> with the passed datetime as its CachedAt value.</returns>
    public static ScheduleWebModel ToWebModel(this Schedule schedule, DateTime cachedAt)
    {
        return new ScheduleWebModel(schedule.Id, cachedAt.ToString("o"), schedule.Days);
    }

    public static DayWebModel ToWebModel(this Day day)
    {
        return new DayWebModel(day.Name, day.Date.ToString("dd/MM"), day.Date.Year, day.Date.Month, day.Date.Day, (int)day.Date.DayOfWeek, day.WeekNumber, day.Events);
    }

    public static UserEventCollection? ToWebModel(this Dictionary<string, List<UserEvent>> userEvents)
    {
        if (!userEvents.ContainsKey("upcoming") || !userEvents.ContainsKey("registered") || !userEvents.ContainsKey("unregistered")) return null;

        return new UserEventCollection(userEvents["upcoming"].Cast<UpcomingUserEvent>().ToList(), userEvents["registered"].Cast<AvailableUserEvent>().ToList(), userEvents["unregistered"].Cast<AvailableUserEvent>().ToList());
    }
}