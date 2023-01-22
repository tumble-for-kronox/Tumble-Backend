using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.RequestModels;
using DatabaseAPI;
using static TumbleBackend.Library.ScheduleManagement;
using KronoxAPI.Exceptions;
using TumbleBackend.StringConstants;
using TumbleBackend.ActionFilters;
using Microsoft.AspNetCore.Cors;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[ServiceFilter(typeof(AuthActionFilter))]
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
    /// <returns></returns>
    [HttpGet("{scheduleId}", Name = "GetSchedule")]
    public IActionResult Get([FromServices] IConfiguration config, [FromRoute] string scheduleId, [FromQuery] SchoolEnum schoolId, [FromQuery] string? startDateISO = null)
    {
        // Extract school instance and make sure the school entry is valid (should've failed in query, but double safety.
        School? school = schoolId.GetSchool();
        Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        if (school.LoginRequired && string.IsNullOrWhiteSpace(sessionToken))
            return BadRequest(new Error($"Login required to access {school.Id} schedules."));

        try
        {
            // If given specific start date that parses correctly, simply fetch schedule directly from KronoxAPI and return it.
            if (startDateISO != null && DateTime.TryParse(startDateISO, out DateTime startDate))
                return Ok(BuildWebSafeSchedule(scheduleId, school, startDate, sessionToken));

            // Reset the given start date as it's either null or an invalid format. Ensures that all cached schedules start at the beginning of the week.
            startDate = DateTime.Now.FirstDayOfWeek();

            // Attempt to get cached schedule.
            ScheduleWebModel? cachedSchedule = SchedulesCache.GetSchedule(scheduleId);

            // On cache hit.
            if (cachedSchedule != null)
            {
                // Make sure cache TTL isn't passed.
                if (Math.Abs(cachedSchedule.CachedAt.Subtract(DateTime.Now).TotalSeconds) >= int.Parse(config[AppSettings.ScheduleCacheTTL]))
                {
                    // Fetch and re-cache schedule if TTL has passed, making sure not to override/change course colors 
                    ScheduleWebModel scheduleFetchForRecache = BuildWebSafeSchedule(scheduleId, school, startDate, sessionToken);
                    SchedulesCache.UpdateSchedule(scheduleId, scheduleFetchForRecache);
                }

                // Return schedule (at this point up-to-date).
                return Ok(cachedSchedule);
            }

            // On cache miss, fetch, cache, and return schedule from scratch.
            ScheduleWebModel newScheduleFetch = BuildWebSafeSchedule(scheduleId, school, startDate, sessionToken);
            SchedulesCache.SaveSchedule(newScheduleFetch);

            return Ok(newScheduleFetch);
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return NotFound(new Error("Schedule wasn't found or may have been corrupted."));
        }
    }

    /// <summary>
    /// Endpoint for fetching multiple given schedules as one. Will circumvent all caching.
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="schoolId"></param>
    /// <param name="startDateISO"></param>
    /// <returns></returns>
    [HttpGet("multi")]
    public IActionResult GetMulti([FromQuery] string[] scheduleIds, [FromQuery] SchoolEnum schoolId, [FromQuery] string? startDateISO = null)
    {
        // Extract school instance and make sure the school entry is valid (should've failed in query, but double safety.
        School? school = schoolId.GetSchool();
        Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        if (school.LoginRequired && string.IsNullOrWhiteSpace(sessionToken))
            return BadRequest(new Error($"Login required to access {school.Id} schedules."));

        try
        {
            // If given specific start date that parses correctly, simply fetch schedule directly from KronoxAPI and return it.
            if (startDateISO != null && DateTime.TryParse(startDateISO, out DateTime startDate))
                return Ok(BuildWebSafeMultiSchedule(scheduleIds, school, startDate, sessionToken));

            // Reset the given start date as it's either null or an invalid format. Ensures that all cached schedules start at the beginning of the week.
            startDate = DateTime.Now.FirstDayOfWeek();

            MultiScheduleWebModel newScheduleFetch = BuildWebSafeMultiSchedule(scheduleIds, school, startDate, sessionToken);

            return Ok(newScheduleFetch);
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
    public IActionResult Search([FromQuery] string searchQuery, [FromQuery] SchoolEnum? schoolId = null)
    {
        School? school = schoolId?.GetSchool();
        Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (school == null)
            return BadRequest(new Error("Invalid school value."));
        try
        {
            List<Programme> searchResult = school.SearchProgrammes(searchQuery, sessionToken);
            HashSet<string> filter = ProgrammeFilters.GetProgrammeFilter(school);

            if (searchResult.Count <= 0)
                return NoContent();

            searchResult.RemoveAll(programme => filter.Contains(programme.Id));

            return Ok(new { searchResult.Count, Items = searchResult });
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Invalid credentials, please login again."));
        }
    }
}
