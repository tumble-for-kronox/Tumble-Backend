using KronoxAPI.Model.Scheduling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels.ResponseModels;

namespace WebAPIModels.Extensions;

public static class DayExtensions
{
    public static DayWebModel ToWebModel(this Day day, Dictionary<string, CourseWebModel> convertedCourses)
    {
        List<EventWebModel> convertedEvents = day.Events.Select(e => e.ToWebModel(convertedCourses[e.Course.Id])).ToList();

        return new DayWebModel(day.Name, day.Date.ToString("d/M", CultureInfo.InvariantCulture), day.Date.ToString("o", CultureInfo.InvariantCulture), day.WeekNumber, convertedEvents);
    }
}
