using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using WebAPIModels.RequestModels;
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
[SessionTokenActionFilter]
[Route("users/events")]
[Route("api/users/events")]
public class UserEventController : ControllerBase
{
    private readonly ILogger<UserEventController> _logger;

    public UserEventController(ILogger<UserEventController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllUserEvents([FromQuery] SchoolEnum schoolId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = await school.GetUserEvents(kronoxReqClient);
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
        //catch (HttpRequestException e)
        //{
        //    _logger.LogError(e.ToString());
        //    return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));
        //}
        catch (TaskCanceledException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("Connection to kronox timed out"));
        }
    }

    [HttpPut("register/all")]
    public async Task<IActionResult> RegisterAllAvailableResults([FromQuery] SchoolEnum schoolId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = await school.GetUserEvents(kronoxReqClient);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();

            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            if (!webSafeUserEvents.UnregisteredEvents.Any())
                return NotFound(new Error("No unregistered events."));

            List<AvailableUserEvent> failedRegistrations = new();
            List<AvailableUserEvent> successfulRegistrations = new();
            foreach (AvailableUserEvent availableUserEvent in webSafeUserEvents.UnregisteredEvents)
            {
                if (!await availableUserEvent.Register(kronoxReqClient))
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
        catch (TaskCanceledException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("Connection to kronox timed out"));
        }
    }

    [HttpPut("register/{eventId}")]
    public async Task<IActionResult> RegisterUserEvent([FromRoute] string eventId, [FromQuery] SchoolEnum schoolId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = await school.GetUserEvents(kronoxReqClient);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();
            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            foreach (AvailableUserEvent userEvent in webSafeUserEvents.UnregisteredEvents)
            {
                if (userEvent.Id == eventId)
                {
                    if (await userEvent.Register(kronoxReqClient))
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
        catch (TaskCanceledException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("Connection to kronox timed out"));
        }
    }

    [HttpPut("unregister/{eventId}")]
    public async Task<IActionResult> UnregisterUserEvent([FromRoute] string eventId, [FromQuery] SchoolEnum schoolId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            Dictionary<string, List<UserEvent>> userEvents = await school.GetUserEvents(kronoxReqClient);
            UserEventCollection? webSafeUserEvents = userEvents.ToWebModel();
            if (webSafeUserEvents == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("We're having trouble getting your data from Kronox, please try again later."));

            foreach (AvailableUserEvent userEvent in webSafeUserEvents.RegisteredEvents)
            {
                if (userEvent.Id == eventId)
                {
                    if (await userEvent.Unregister(kronoxReqClient))
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
        catch (TaskCanceledException e)
        {
            _logger.LogError(e.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("Connection to kronox timed out"));
        }
    }

}
