using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.Extensions;
using KronoxAPI.Model.Users;
using KronoxAPI.Exceptions;
using TumbleBackend.ActionFilters;
using Microsoft.AspNetCore.Cors;
using TumbleHttpClient;
using TumbleBackend.StringConstants;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[KronoxUrlFilter]
[ServiceFilter(typeof(AuthActionFilter))]
[Route("api/users/events")]
public class UserEventController : ControllerBase
{
    private readonly ILogger<UserEventController> _logger;

    public UserEventController(ILogger<UserEventController> logger)
    {
        _logger = logger;
    }

    private static IKronoxRequestClient GetAuthenticatedClient()
    {
        if (HttpContext.Items[KronoxReqClientKeys.SingleClient] is not IKronoxRequestClient client || !client.IsAuthenticated)
        {
            throw new UnauthorizedAccessException("Requires provided auth token");
        }
        return client;
    }

    private IActionResult HandleError(Exception ex, string message, int statusCode = StatusCodes.Status500InternalServerError)
    {
        _logger.LogError(ex.ToString());
        return StatusCode(statusCode, new Error(message));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUserEvents([FromQuery] SchoolEnum schoolId)
    {
        try
        {
            var kronoxReqClient = GetAuthenticatedClient();
            var userEvents = await School.GetUserEventsAsync(kronoxReqClient);
            var webSafeUserEvents = userEvents?.ToWebModel();

            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            return Ok(webSafeUserEvents);
        }
        catch (Exception ex) when (ex is ParseException or LoginException or HttpRequestException)
        {
            return HandleError(ex, "We're having trouble getting your data from Kronox, please try again later.");
        }
    }

    [HttpPut("register/all")]
    public async Task<IActionResult> RegisterAllAvailableResults([FromQuery] SchoolEnum schoolId)
    {
        try
        {
            var kronoxReqClient = GetAuthenticatedClient();
            var userEvents = await School.GetUserEventsAsync(kronoxReqClient);
            var webSafeUserEvents = userEvents?.ToWebModel();

            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            if (!webSafeUserEvents.UnregisteredEvents.Any())
                return NotFound(new Error("No unregistered events."));

            var (successfulRegistrations, failedRegistrations) = await RegisterEventsAsync(kronoxReqClient, webSafeUserEvents.UnregisteredEvents);

            return Ok(new MultiRegistrationResult(successfulRegistrations, failedRegistrations));
        }
        catch (Exception ex) when (ex is ParseException or LoginException or HttpRequestException)
        {
            return HandleError(ex, "We're having trouble getting your data from Kronox, please try again later.");
        }
    }

    [HttpPut("register/{eventId}")]
    public async Task<IActionResult> RegisterUserEvent([FromRoute] string eventId, [FromQuery] SchoolEnum schoolId)
    {
        try
        {
            var kronoxReqClient = GetAuthenticatedClient();
            var userEvents = await School.GetUserEventsAsync(kronoxReqClient);
            var webSafeUserEvents = userEvents?.ToWebModel();

            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            var eventToRegister = webSafeUserEvents.UnregisteredEvents.FirstOrDefault(e => e.Id == eventId);

            if (eventToRegister == null)
                return NotFound(new Error("The event specified couldn't be found in Kronox's system, please log out and back in or try again later."));

            if (await eventToRegister.Register(kronoxReqClient))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an error signing up to the event, please try again later."));
        }
        catch (Exception ex) when (ex is ParseException or LoginException or HttpRequestException)
        {
            return HandleError(ex, "We're having trouble getting your data from Kronox, please try again later.");
        }
    }

    [HttpPut("unregister/{eventId}")]
    public async Task<IActionResult> UnregisterUserEvent([FromRoute] string eventId, [FromQuery] SchoolEnum schoolId)
    {
        try
        {
            var kronoxReqClient = GetAuthenticatedClient();
            var userEvents = await School.GetUserEventsAsync(kronoxReqClient);
            var webSafeUserEvents = userEvents?.ToWebModel();

            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            var eventToUnregister = webSafeUserEvents.RegisteredEvents.FirstOrDefault(e => e.Id == eventId);

            if (eventToUnregister == null)
                return NotFound(new Error("The event specified couldn't be found in Kronox's system, please log out and back in or try again later."));

            if (await eventToUnregister.Unregister(kronoxReqClient))
                return Ok();

            return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an error signing up to the event, please try again later."));
        }
        catch (Exception ex) when (ex is ParseException or LoginException or HttpRequestException)
        {
            return HandleError(ex, "We're having trouble getting your data from Kronox, please try again later.");
        }
    }

    private async Task<(List<AvailableUserEvent> successful, List<AvailableUserEvent> failed)> RegisterEventsAsync(IKronoxRequestClient client, IEnumerable<AvailableUserEvent> events)
    {
        var successful = new List<AvailableUserEvent>();
        var failed = new List<AvailableUserEvent>();

        foreach (var userEvent in events)
        {
            if (await userEvent.Register(client))
                successful.Add(userEvent);
            else
                failed.Add(userEvent);
        }

        return (successful, failed);
    }
}
