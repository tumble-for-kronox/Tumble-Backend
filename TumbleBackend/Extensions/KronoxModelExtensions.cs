using WebAPIModels;
using KronoxAPI.Model.Scheduling;

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
}