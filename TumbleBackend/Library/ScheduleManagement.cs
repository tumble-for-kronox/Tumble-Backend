using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Schools;
using TumbleBackend.Utilities;
using TumbleBackend.Extensions;
using WebAPIModels.Extensions;
using WebAPIModels.ResponseModels;
using KronoxAPI.Exceptions;

namespace TumbleBackend.Library;

public class ScheduleManagement
{
    /// <summary>
    /// Method for fetching a schedule, generating colors for courses, and bundling courses into a dictionary. Used when a schedule is needed as "built from scratch".
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="school"></param>
    /// <param name="startDate"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    /// <exception cref="ParseException"></exception>
    public static ScheduleWebModel BuildWebSafeSchedule(string scheduleId, School school, DateTime startDate, string? sessionToken)
    {
        try
        {
            Schedule schedule = school.FetchSchedule(scheduleId, null, sessionToken, startDate);

            Dictionary<string, CourseWebModel> courses = schedule.Courses().Select((kvp, index) => kvp.Value.ToWebModel(TranslatorUtil.SwedishToEnglish(kvp.Value.Name).Result)).ToDictionary(course => course.Id);
            ScheduleWebModel webSafeSchedule = schedule.ToWebModel(courses);
            webSafeSchedule.Days = webSafeSchedule.Days.PadScheduleDays(startDate);

            return webSafeSchedule;
        }
        catch (ParseException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}
