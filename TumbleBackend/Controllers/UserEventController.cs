using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using WebAPIModels.RequestModels;
using TumbleBackend.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.Extensions;
using KronoxAPI.Model.Users;
using KronoxAPI.Exceptions;
using System.Web.Http.Cors;
using TumbleBackend.ActionFilters;

namespace TumbleBackend.Controllers;

[EnableCors(origins: "*", headers: "*", methods: "*")]
[ApiController]
[ServiceFilter(typeof(AuthActionFilter))]
[Route("users/events")]
public class UserEventController : ControllerBase
{
    private readonly ILogger<UserEventController> _logger;

    public UserEventController(ILogger<UserEventController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAllUserEvents([FromQuery] SchoolEnum schoolId)
    {
        School? school = schoolId.GetSchool();
        bool hasSessionToken = Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (!hasSessionToken)
            return BadRequest(new Error("Requires provided auth token"));

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = school.GetUserEvents(sessionToken);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();
            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            return Ok(webSafeUserEvents);
        }
        catch (ParseException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
        catch (LoginException e)
        {
            _logger.LogError(e.ToString());
            return Unauthorized(new Error("There was an issue with your login information, please log back in or try again later."));
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
    }

    [HttpPut("register/all")]
    public IActionResult RegisterAllAvailableResults([FromQuery] SchoolEnum schoolId)
    {
        School? school = schoolId.GetSchool();
        bool hasSessionToken = Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (!hasSessionToken)
            return BadRequest(new Error("Requires provided auth token"));

        if (school == null)
        {
            return BadRequest(new Error("Invalid school value."));
        }

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = school.GetUserEvents(sessionToken);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();

            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            if (!webSafeUserEvents.UnregisteredEvents.Any())
                return NotFound(new Error("No unregistered events."));

            List<AvailableUserEvent> failedRegistrations = new();
            List<AvailableUserEvent> successfulRegistrations = new();
            foreach (AvailableUserEvent availableUserEvent in webSafeUserEvents.UnregisteredEvents)
            {
                if (!availableUserEvent.Register(school, sessionToken))
                    failedRegistrations.Add(availableUserEvent);
                else
                    successfulRegistrations.Add(availableUserEvent);
            }

            return Ok(new MultiRegistrationResult(successfulRegistrations, failedRegistrations));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
        catch (LoginException e)
        {
            _logger.LogError(e.ToString());
            return Unauthorized(new Error("There was an issue with your login information, please log back in or try again later."));
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
    }

    [HttpPut("register/{eventId}")]
    public IActionResult RegisterUserEvent([FromRoute] string eventId, [FromQuery] SchoolEnum schoolId)
    {
        School? school = schoolId.GetSchool();
        bool hasSessionToken = Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (!hasSessionToken)
            return BadRequest(new Error("Requires provided auth token"));

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = school.GetUserEvents(sessionToken);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();
            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            foreach (AvailableUserEvent userEvent in webSafeUserEvents.UnregisteredEvents)
            {
                if (userEvent.Id == eventId)
                {
                    if (userEvent.Register(school, sessionToken))
                        return Ok();

                    return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an error signing up to the event, please try again later."));
                }
            }

            return NotFound(new Error("The event specified couldn't be found in Kronox's system, please log out and back in or try again later."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
        catch (LoginException e)
        {
            _logger.LogError(e.ToString());
            return Unauthorized(new Error("There was an issue with your login information, please log back in or try again later."));
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
    }

    [HttpPut("unregister/{eventId}")]
    public IActionResult UnregisterUserEvent([FromRoute] string eventId, [FromQuery] SchoolEnum schoolId)
    {
        School? school = schoolId.GetSchool();
        bool hasSessionToken = Request.Headers.TryGetValue("sessionToken", out var sessionToken);

        if (!hasSessionToken)
            return BadRequest(new Error("Requires provided auth token"));

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = school.GetUserEvents(sessionToken);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();
            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            foreach (AvailableUserEvent userEvent in webSafeUserEvents.RegisteredEvents)
            {
                if (userEvent.Id == eventId)
                {
                    if (userEvent.Unregister(school, sessionToken))
                        return Ok();

                    return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an error signing up to the event, please try again later."));
                }
            }

            return NotFound(new Error("The event specified couldn't be found in Kronox's system, please log out and back in or try again later."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
        catch (LoginException e)
        {
            _logger.LogError(e.ToString());
            return Unauthorized(new Error("There was an issue with your login information, please log back in or try again later."));
        }
        catch (HttpRequestException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        }
    }

}
