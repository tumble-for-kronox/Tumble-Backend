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
using TumbleHttpClient;
using KronoxAPI.Extensions;
using Utilities.Pair;
using DatabaseAPI.Interfaces;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[KronoxUrlFilter]
[Route("api/schedules")]
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
    [ServiceFilter(typeof(AuthActionFilter))]
    [HttpGet("{scheduleId}")]
    public async Task<IActionResult> GetSingleSchedule([FromServices] IConfiguration config, [FromServices] IDbSchedulesService schedulesService, [FromRoute] string scheduleId, [FromQuery] SchoolEnum schoolId, [FromQuery] string? startDateISO = null)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        // Extract school instance and make sure the school entry is valid (should've failed in query, but double safety.)
        School school = schoolId.GetSchool()!;

        if (school.LoginRequired && !kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error($"Login required to access {school.Id.ToStringId()} schedules."));

        try
        {
            // If given specific start date that parses correctly, simply fetch schedule directly from KronoxAPI and return it.
            if (startDateISO != null && DateTime.TryParse(startDateISO, out DateTime startDate))
                return Ok(BuildWebSafeSchedule(kronoxReqClient, scheduleId, school, startDate));

            // Reset the given start date as it's either null or an invalid format. Ensures that all cached schedules start at the beginning of the week.
            startDate = DateTime.Now.FirstDayOfWeek();

            // Attempt to get cached schedule.
            ScheduleWebModel? cachedSchedule = await schedulesService.GetScheduleAsync(scheduleId);

            // On cache hit.
            if (cachedSchedule != null)
            {
                // Make sure cache TTL isn't passed.
                if (Math.Abs(cachedSchedule.CachedAt.Subtract(DateTime.Now).TotalSeconds) >= int.Parse(config[AppSettings.ScheduleCacheTTL]))
                {
                    // Fetch and re-cache schedule if TTL has passed, making sure not to override/change course colors 
                    ScheduleWebModel scheduleFetchForRecache = await BuildWebSafeSchedule(kronoxReqClient, scheduleId, school, startDate);
                    await schedulesService.UpsertScheduleAsync(scheduleFetchForRecache);
                }

                // Return schedule (at this point up-to-date).
                return Ok(cachedSchedule);
            }

            // On cache miss, fetch, cache, and return schedule from scratch.
            ScheduleWebModel newScheduleFetch = await BuildWebSafeSchedule(kronoxReqClient, scheduleId, school, startDate);
            await schedulesService.UpsertScheduleAsync(newScheduleFetch);

            return Ok(newScheduleFetch);
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return NotFound(new Error("Schedule wasn't found or may have been corrupted."));
        }
    }

    /// <summary>
    /// Retrieves events for multiple schools and schedules based on the provided parameters.
    /// </summary>
    /// <param name="schoolSchedules">An array of MultiSchoolSchedules objects that includes school IDs and schedule IDs to retrieve events from. This parameter is required.</param>
    /// <param name="n_events">An integer that specifies the number of events to retrieve. Default value is 1. This parameter is optional.</param>
    /// <param name="startDateISO">A string that specifies the starting date for the events to retrieve in ISO 8601 format. This parameter is optional.</param>
    /// <returns>
    /// Returns an IActionResult object that represents the result of the action.
    /// It returns an HTTP 400 Bad Request response if an invalid school value is provided.
    /// It returns an HTTP 404 Not Found response if a schedule wasn't found or may have been corrupted.
    /// If the retrieval of events is successful, it returns an HTTP 200 OK response with the retrieved events as a JSON array.
    /// </returns>
    /// <exception cref="ParseException">Thrown when there is an error while parsing the start date in ISO 8601 format.</exception>
    [HttpPost("nevents")]
    public async Task<IActionResult> GetEvents([FromBody] MultiSchoolSchedules[] schoolSchedules, [FromQuery] int n_events = 1, [FromQuery] string? startDateISO = null)
    {
        IEnumerable<IPair<SchoolEnum, IKronoxRequestClient>> kronoxReqClients = (HttpContext.Items[KronoxReqClientKeys.MultiClient] as IEnumerable<Pair<SchoolEnum, KronoxRequestClient>>)!.CastPairs<SchoolEnum, IKronoxRequestClient>();
        MultiScheduleWebModel finalSchedule;

        try
        {
            if (startDateISO != null && DateTime.TryParse(startDateISO, out var startDate))
            {
                finalSchedule = await BuildWebSafeMultiSchoolSchedule(kronoxReqClients, schoolSchedules, startDate);
                return Ok(finalSchedule.GetEvents().Take(n_events));
            }

            startDate = DateTime.Now;
            finalSchedule = await BuildWebSafeMultiSchoolSchedule(kronoxReqClients, schoolSchedules, startDate);
            return Ok(finalSchedule.GetEvents().Take(n_events));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return NotFound(new Error("Schedule wasn't found or may have been corrupted."));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new Error("Invalid school value."));
        }
    }

    /// <summary>
    /// Endpoint for fetching multiple given schedules as one. Will circumvent all caching.
    /// </summary>
    /// <param name="schoolSchedules"></param>
    /// <param name="startDateISO"></param>
    /// <returns></returns>
    [HttpPost("multi")]
    public async Task<IActionResult> GetMulti([FromBody] MultiSchoolSchedules[] schoolSchedules, [FromQuery] string? startDateISO = null)
    {
        IEnumerable<IPair<SchoolEnum, IKronoxRequestClient>> kronoxReqClients = (HttpContext.Items[KronoxReqClientKeys.MultiClient] as IEnumerable<Pair<SchoolEnum, KronoxRequestClient>>)!.CastPairs<SchoolEnum, IKronoxRequestClient>();
        try
        {
            if (startDateISO != null && DateTime.TryParse(startDateISO, out var startDate))
            {
                return Ok(await BuildWebSafeMultiSchoolSchedule(kronoxReqClients, schoolSchedules, startDate));
            }

            startDate = DateTime.Now;
            return Ok(await BuildWebSafeMultiSchoolSchedule(kronoxReqClients, schoolSchedules, startDate));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return NotFound(new Error("Schedule wasn't found or may have been corrupted."));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new Error("Invalid school value."));
        }
    }

    /// <summary>
    /// Endpoint for searching Kronox's website for programmes/schedules/people.
    /// </summary>
    /// <param name="searchQuery"></param>
    /// <param name="schoolId"></param>
    /// <param name="sessionToken"></param>
    /// <returns>A list of <see cref="Programme"/> objects and an <see cref="int"/> Count. Although the name is "programme" they also map to individuals and schedules correctly.</returns>
    [HttpGet("search")]
    [ServiceFilter(typeof(AuthActionFilter))]
    public async Task<IActionResult> Search([FromServices] IDbProgrammeFiltersService programmeFiltersService, [FromQuery] string searchQuery, [FromQuery] SchoolEnum? schoolId = null)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School? school = schoolId?.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));
        try
        {
            List<Programme> searchResult = await school.SearchProgrammes(kronoxReqClient, searchQuery);
            HashSet<string> filter = await programmeFiltersService.GetProgrammeFiltersAsync(school);

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
