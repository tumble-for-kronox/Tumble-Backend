using KronoxAPI.Exceptions;
using KronoxAPI.Model.Booking;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Web.Http.Cors;
using TumbleBackend.Extensions;
using WebAPIModels.RequestModels;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.Controllers;

[EnableCors(origins: "*", headers: "*", methods: "*")]
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
    public ActionResult<List<Resource>> GetResources([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken)
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
    public ActionResult<List<Booking>> GetUserBookings([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken)
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
    public ActionResult<Resource> GetAvailabilities([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromRoute] string resourceId, [FromQuery] DateTime date)
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
    public async Task<ActionResult> BookResource([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromBody] BookingRequest bookingRequest)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
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
            return StatusCode(StatusCodes.Status409Conflict, new Error("Couldn't book resource because of a conflict. You may already have a room booked at the same time."));
        }
        catch (MaxBookingsException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status403Forbidden, new Error("Couldn't book resource because you already have the max number of allowed bookings."));
        }
    }

    [HttpPut("unbook")]
    public async Task<ActionResult> UnbookResource([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromQuery] string bookingId)
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
            return StatusCode(StatusCodes.Status404NotFound, new Error("Couldn't unbook resource because you don't have the bookingId."));
        }
    }

    [HttpPut("confirm")]
    public async  Task<ActionResult> ConfirmResourceBooking([FromQuery] SchoolEnum schoolId, [FromQuery] string sessionToken, [FromBody] ConfirmBookingRequest data)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            await school.Resources.ConfirmResourceBooking(sessionToken, data.BookingId, data.ResourceId);

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
            return StatusCode(StatusCodes.Status404NotFound, new Error("Couldn't unbook resource because you don't have the bookingId."));
        }
    }
}
