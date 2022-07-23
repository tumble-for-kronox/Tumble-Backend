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
            string[] courseColors = CourseColorUtil.GetScheduleColors(schedule.Courses.Count);

            Dictionary<string, CourseWebModel> courses = schedule.Courses.Select((kvp, index) => kvp.Value.ToWebModel(courseColors[index], TranslatorUtil.SwedishToEnglish(kvp.Value.Name).Result)).ToDictionary(course => course.Id);
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
    /// A utility method used for concatenating two dicts of relatively <see cref="Dictionary{string, CourseWebModel}"/> and <see cref="Dictionary{string, Course}"/> into one, while generating new colors only for the courses that needs it.
    /// <para>
    /// Lot of side effects including: populating new courses with random colors and removing courses from dict1 that are not present in dict2. Should only be used when re-caching an already cached schedule with new data.
    /// </para>
    /// </summary>
    /// <param name="dict1"></param>
    /// <param name="dict2"></param>
    /// <returns>Concatenated <see cref="Dictionary{string, CourseWebModel}"/> containing the updated and combined course information from both input dicts.</returns>
    public static Dictionary<string, CourseWebModel> ConcatCourseDataForReCache(Dictionary<string, CourseWebModel> dict1, Dictionary<string, Course> dict2)
    {
        List<string> oldColors = new();
        List<string> keysToRemoveFromDict1 = new();
        foreach (string key in dict1.Keys)
        {
            if (!dict2.ContainsKey(key))
            {
                keysToRemoveFromDict1.Add(key);
                continue;
            }

            dict2.Remove(key);
            oldColors.Add(dict1[key].Color);
        }

        string[] newColors = new string[dict2.Count].Concat(oldColors.ToArray()).ToArray();
        for (int i = oldColors.Count; i < oldColors.Count + dict2.Count; i++)
        {
            string curColor = CourseColorUtil.GetSingleRandColor();

            while (newColors.Contains(curColor))
                curColor = CourseColorUtil.GetSingleRandColor();
        }

        Dictionary<string, CourseWebModel> dict2Converted = dict2.Select((kvp, i) => kvp.Value.ToWebModel(newColors[i], TranslatorUtil.SwedishToEnglish(kvp.Value.Name).Result)).ToDictionary(course => course.Id);

        return dict1.Concat(dict2Converted).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }
}
