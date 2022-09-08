using KronoxAPI.Exceptions;
using KronoxAPI.Model.Booking;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Extensions;
using WebAPIModels.RequestModels;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.Controllers;

[ApiController]
[Route("resources")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;

    public BookingController(ILogger<BookingController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetResources([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            return Ok(school.Resources.GetResources(sessionToken));
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Username or password incorrect."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while attempting to parse school resources, please try again later."));
        }
    }

    [HttpGet("userbookings")]
    public IActionResult GetUserBookings([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            return Ok(school.Resources.GetUserBookings(sessionToken));
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Username or password incorrect."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while logging in, please try again later."));
        }
    }

    [HttpGet("{resourceId}")]
    public IActionResult GetAvailabilities([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromRoute] string resourceId, [FromQuery] DateTime date)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            List<Resource> resources = school.Resources.GetResources(sessionToken);
            Resource resource = resources.Where(e => e.Id == resourceId).First().FetchData(school.Url, sessionToken, date);
            return Ok(resource);
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Username or password incorrect."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while attempting to parse resource availability, please try again later."));
        }
        catch (ResourceInavailableException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status404NotFound, new Error("The resourece you attempted to access is completely inavailable. This may be because you are trying to access it on a weekend."));
        }
    }

    [HttpPut("book")]
    public async Task<IActionResult> BookResource([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromBody] BookingRequest bookingRequest)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            List<Booking> userBookings = school.Resources.GetUserBookings(sessionToken);

            await school.Resources.BookResource(sessionToken, bookingRequest.ResourceId, bookingRequest.Date, bookingRequest.Slot);

            return Ok();
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Username or password incorrect."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while logging in, please try again later."));
        }
        catch (BookingCollisionException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status409Conflict, new Error("Couldn't book resource because of a conflict. It may already be booked or unavaiable."));
        }
        catch (MaxBookingsException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status403Forbidden, new Error("Couldn't book resource because of a conflict. It may already be booked or unavaiable."));
        }
    }

    [HttpPut("unbook")]
    public async Task<IActionResult> UnbookResource([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromQuery] string bookingId)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            await school.Resources.UnbookResource(sessionToken, bookingId);

            return Ok();
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Username or password incorrect."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while attempting to unbook a resource, please try again later."));
        }
        catch (BookingCollisionException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status409Conflict, new Error("Couldn't book resource because of a conflict. It may already be booked or unavaiable."));
        }
    }
}
