using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Schools;
using TumbleBackend.Utilities;
using TumbleBackend.Extensions;
using WebAPIModels.Extensions;
using WebAPIModels.ResponseModels;
using KronoxAPI.Exceptions;
using WebAPIModels.RequestModels;
using TumbleHttpClient;
using Utilities.Pair;

namespace TumbleBackend.Library;

public class ScheduleManagement
{
    /// <summary>
    /// Method for fetching a schedule, and bundling courses into a dictionary. Used when a schedule is needed as "built from scratch".
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="school"></param>
    /// <param name="startDate"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    /// <exception cref="ParseException"></exception>
    public static async Task<ScheduleWebModel> BuildWebSafeSchedule(IKronoxRequestClient client, string scheduleId, School school, DateTime startDate)
    {
        try
        {
            Schedule schedule = await school.FetchSchedule(client, new string[] { scheduleId }, null, startDate);

            //Dictionary<string, CourseWebModel> courses = schedule.Courses().Select((kvp, index) => kvp.Value.ToWebModel(TranslatorUtil.SwedishToEnglish(kvp.Value.Name).Result)).ToDictionary(course => course.Id);
            Dictionary<string, CourseWebModel> courses = schedule.Courses().Select((kvp, index) => kvp.Value.ToWebModel(kvp.Value.Name)).ToDictionary(course => course.Id);
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

    /// <summary>
    /// Method for fetching a set of schedules, and bundling courses into a dictionary. Used when a schedule is needed as "built from scratch".
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="school"></param>
    /// <param name="startDate"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    /// <exception cref="ParseException"></exception>
    public static async Task<MultiScheduleWebModel> BuildWebSafeMultiSchedule(IKronoxRequestClient client, string[] scheduleIds, School school, DateTime startDate)
    {
        try
        {
            Schedule schedule = await school.FetchSchedule(client, scheduleIds, null, startDate);

            //Dictionary<string, CourseWebModel> courses = schedule.Courses().Select((kvp, index) => kvp.Value.ToWebModel(TranslatorUtil.SwedishToEnglish(kvp.Value.Name).Result)).ToDictionary(course => course.Id);
            Dictionary<string, CourseWebModel> courses = schedule.Courses().Select((kvp, index) => kvp.Value.ToWebModel(kvp.Value.Name)).ToDictionary(course => course.Id);
            MultiScheduleWebModel webSafeSchedule = schedule.ToMultiWebModel(courses);
            webSafeSchedule.Days = webSafeSchedule.Days.PadScheduleDays(startDate);

            return webSafeSchedule;
        }
        catch (ParseException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public static async Task<MultiScheduleWebModel> BuildWebSafeMultiSchoolSchedule(IEnumerable<IPair<SchoolEnum, IKronoxRequestClient>> clients, MultiSchoolSchedules[] schoolsAndSchedules, DateTime startDate)
    {
        List<MultiScheduleWebModel> schedules = new();

        foreach (var schoolAndSchedules in schoolsAndSchedules)
        {
            School? school = schoolAndSchedules.SchoolId.GetSchool() ?? throw new ArgumentException("School value is invalid.");
            IKronoxRequestClient client = clients.First(client => client.Key == school.Id).Value;

            try
            {
                schedules.Add(await BuildWebSafeMultiSchedule(client, schoolAndSchedules.ScheduleIds, school, startDate));
            }
            catch (ParseException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        return schedules.CombineAll();
    }
}
