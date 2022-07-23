using KronoxAPI.Model.Scheduling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.ResponseModels;

namespace WebAPIModels.Extensions;

public static class ScheduleExtensions
{
    /// <summary>
    /// Create a Web Model object from a given <see cref="Schedule"/> object with the current time as the CachedAt value.
    /// </summary>
    /// <param name="schedule"></param>
    /// <returns>A <see cref="ScheduleWebModel"/> with the current time as its CachedAt value.</returns>
    public static ScheduleWebModel ToWebModel(this Schedule schedule, Dictionary<string, CourseWebModel> convertedCourses)
    {
        List<DayWebModel> convertedDays = schedule.Days.Select(day => day.ToWebModel()).ToList();

        return new ScheduleWebModel(schedule.Id, DateTime.Now, convertedDays, convertedCourses);
    }

    /// <summary>
    /// Create a Web Model object from a given <see cref="Schedule"/> object with the given <paramref name="cachedAt"/> datetime as the CachedAt value.
    /// </summary>
    /// <param name="schedule"></param>
    /// <param name="cachedAt"></param>
    /// <returns>A <see cref="ScheduleWebModel"/> with the passed datetime as its CachedAt value.</returns>
    public static ScheduleWebModel ToWebModel(this Schedule schedule, DateTime cachedAt, Dictionary<string, CourseWebModel> convertedCourses)
    {
        List<DayWebModel> convertedDays = schedule.Days.Select(day => day.ToWebModel()).ToList();

        return new ScheduleWebModel(schedule.Id, cachedAt, convertedDays, convertedCourses);
    }
}
