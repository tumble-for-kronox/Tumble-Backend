using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Schools;
using KronoxAPI.Utilities;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Utilities;
using TumbleBackend.Extensions;
using WebAPIModels.Extensions;
using WebAPIModels;
using DatabaseAPI;

namespace TumbleBackend.Controllers;

[ApiController]
[Route("schedules")]
public class ScheduleController : ControllerBase
{
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(ILogger<ScheduleController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Endpoint for fetching a given schedule. Will attempt to fetch from cache first, if cache miss occurs it will fetch and parse from the KronoxAPI and store in cache.
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="schoolId"></param>
    /// <param name="startDateISO"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    [HttpGet("{scheduleId}", Name = "GetSchedule")]
    public IActionResult Get([FromServices] IConfiguration config, [FromRoute] string scheduleId, [FromQuery] SchoolEnum schoolId, [FromQuery] string? startDateISO = null, [FromQuery] string? sessionToken = null)
    {
        // Extract school instance and make sure the school entry is valid (should've failed in query, but double safety.
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        // If given specific start date that parses correctly, simply fetch schedule directly from KronoxAPI and return it.
        if (startDateISO != null && DateTime.TryParse(startDateISO, out DateTime startDate))
            return Ok(BuildWebSafeSchedule(scheduleId, school, startDate, sessionToken));

        // Reset the given start date as it's either null or an invalid format. Ensures that all cached schedules start at the beginning of the week.
        startDate = DateTime.Now.StartOfWeek();

        // Attempt to get cached schedule.
        ScheduleWebModel? cachedSchedule = SchedulesCache.GetSchedule(scheduleId);

        // On cached hit.
        if (cachedSchedule != null)
        {
            // Make sure cache TTL isn't passed.
            if (Math.Abs(cachedSchedule.CachedAt.Subtract(DateTime.Now).TotalSeconds) >= int.Parse(config["CacheTTLInSeconds"]))
            {
                // Fetch and re-cache schedule if TTL has passed, making sure not to override/change course colors 
                Schedule newScheduleData = school.FetchSchedule(scheduleId, null, sessionToken, startDate);
                cachedSchedule = newScheduleData.ToWebModel(ConcatCourseDataForReCache(cachedSchedule.Courses, newScheduleData.Courses));

                SchedulesCache.UpdateSchedule(scheduleId, cachedSchedule);
            }

            // Return schedule (at this point up-to-date).
            return Ok(cachedSchedule);
        }

        // On cache miss, fetch, cache, and return schedule from scratch.
        ScheduleWebModel webSafeSchedule = BuildWebSafeSchedule(scheduleId, school, startDate, sessionToken);
        SchedulesCache.SaveSchedule(webSafeSchedule);

        return Ok(webSafeSchedule);
    }

    /// <summary>
    /// Endpoint for searching Kronox's website for programmes/schedules/people.
    /// </summary>
    /// <param name="searchQuery"></param>
    /// <param name="schoolId"></param>
    /// <param name="sessionToken"></param>
    /// <returns>A list of <see cref="Programme"/> objects and an <see cref="int"/> Count. Although the name is "programme" they also map to individuals and schedules correctly.</returns>
    [HttpGet("search", Name = "SearchProgrammes")]
    public IActionResult Search([FromQuery] string searchQuery, [FromQuery] SchoolEnum schoolId, [FromQuery] string? sessionToken = null)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        List<Programme> searchResult = school.SearchProgrammes(searchQuery, sessionToken);

        return Ok(new { searchResult.Count, Items = searchResult });
    }

    /// <summary>
    /// Method for fetching a schedule, generating colors for courses, and bundling courses into a dictionary. Used when a schedule is needed as "built from scratch".
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="school"></param>
    /// <param name="startDate"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    private static ScheduleWebModel BuildWebSafeSchedule(string scheduleId, School school, DateTime startDate, string? sessionToken)
    {
        Schedule schedule = school.FetchSchedule(scheduleId, null, sessionToken, startDate);
        string[] courseColors = CourseColorUtil.GetScheduleColors(schedule.Courses.Count);

        Dictionary<string, CourseWebModel> courses = schedule.Courses.Select((kvp, index) => kvp.Value.ToWebModel(courseColors[index], TranslatorUtil.SwedishToEnglish(kvp.Value.Name).Result)).ToDictionary(course => course.Id);

        return schedule.ToWebModel(courses);
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
    private static Dictionary<string, CourseWebModel> ConcatCourseDataForReCache(Dictionary<string, CourseWebModel> dict1, Dictionary<string, Course> dict2)
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
