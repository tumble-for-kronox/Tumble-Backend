using KronoxAPI.Model.Scheduling;
using WebAPIModels.ResponseModels;

namespace WebAPIModels.Extensions;

public static class EventExtensions
{
    public static EventWebModel ToWebModel(this Event e, CourseWebModel course)
    {
        return new EventWebModel(e.Id, e.ScheduleId, e.Title, course, e.Teachers, e.TimeStart, e.TimeEnd, e.Locations, e.IsSpecial, e.LastModified);
    }
}
