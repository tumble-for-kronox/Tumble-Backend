using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Extensions;
using WebAPIModels.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.RequestModels;
using DatabaseAPI;
using static TumbleBackend.Library.ScheduleManagement;
using KronoxAPI.Exceptions;

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

        if (school.LoginRequired && sessionToken == null)
            return BadRequest(new Error($"Login required to access {school.Id} schedules."));

        // If given specific start date that parses correctly, simply fetch schedule directly from KronoxAPI and return it.
        if (startDateISO != null && DateTime.TryParse(startDateISO, out DateTime startDate))
            return Ok(BuildWebSafeSchedule(scheduleId, school, startDate, sessionToken));

        // Reset the given start date as it's either null or an invalid format. Ensures that all cached schedules start at the beginning of the week.
        startDate = DateTime.Now.StartOfWeek();

        // Attempt to get cached schedule.
        ScheduleWebModel? cachedSchedule = SchedulesCache.GetSchedule(scheduleId);

        try
        {
            // On cached hit.
            if (cachedSchedule != null)
            {
                // Make sure cache TTL isn't passed.
                if (Math.Abs(cachedSchedule.CachedAt.Subtract(DateTime.Now).TotalSeconds) >= int.Parse(config["CacheTTLInSeconds"]))
                {
                    // Fetch and re-cache schedule if TTL has passed, making sure not to override/change course colors 
                    Schedule newScheduleData = school.FetchSchedule(scheduleId, null, sessionToken, startDate);
                    cachedSchedule = newScheduleData.ToWebModel(ConcatCourseDataForReCache(cachedSchedule.Courses, newScheduleData.Courses));
                    cachedSchedule.Days = cachedSchedule.Days.PadScheduleDays(startDate);
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
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return NotFound(new Error("Schedule wasn't found or may have been corrupted."));
        }
    }

    /// <summary>
    /// Endpoint for searching Kronox's website for programmes/schedules/people.
    /// </summary>
    /// <param name="searchQuery"></param>
    /// <param name="schoolId"></param>
    /// <param name="sessionToken"></param>
    /// <returns>A list of <see cref="Programme"/> objects and an <see cref="int"/> Count. Although the name is "programme" they also map to individuals and schedules correctly.</returns>
    [HttpGet("search", Name = "SearchProgrammes")]
    public IActionResult Search([FromQuery] string searchQuery, [FromQuery] SchoolEnum? schoolId = null, [FromQuery] string? sessionToken = null)
    {
        School? school = schoolId?.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));
        try
        {
            List<Programme> searchResult = school.SearchProgrammes(searchQuery, sessionToken);
            return Ok(new { searchResult.Count, Items = searchResult });
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Invalid credentials, please login again."));
        }
    }
}
