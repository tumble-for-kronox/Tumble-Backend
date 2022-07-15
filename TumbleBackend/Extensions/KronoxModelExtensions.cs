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

    }

    public static ScheduleWebModel ToWebModel(this Schedule schedule, DateTime cachedAt)
    {

    }
}