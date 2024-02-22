using KronoxAPI.Exceptions;
using KronoxAPI.Model.Booking;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.ActionFilters;
using TumbleBackend.Extensions;
using TumbleBackend.StringConstants;
using TumbleHttpClient;
using WebAPIModels.RequestModels;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[KronoxUrlFilter]
[ServiceFilter(typeof(AuthActionFilter))]
[Route("api/resources")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;

    public BookingController(ILogger<BookingController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<List<Resource>>> GetResources([FromQuery] SchoolEnum schoolId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            return Ok(await SchoolResources.GetResourcesAsync(kronoxReqClient));
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

    [HttpGet("all")]
    public async Task<ActionResult<List<Resource>>> GetAllResourcesAndAvailabilities([FromQuery] SchoolEnum schoolId, [FromQuery] DateTime date)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));


        try
        {
            List<Resource> sparseResources = await SchoolResources.GetResourcesAsync(kronoxReqClient);
            IEnumerable<Task<Resource>> fullResourcesTasks = sparseResources.Select(async e => await e.FetchData(kronoxReqClient, date));
            return Ok(await Task.WhenAll(fullResourcesTasks));
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
        catch (ResourceInavailableException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status404NotFound, new Error("The resourece you attempted to access is completely inavailable. This may be because you are trying to access it on a weekend."));
        }

    }

    [HttpGet("userbookings")]
    public async Task<ActionResult<List<Booking>>> GetUserBookings([FromQuery] SchoolEnum schoolId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            List<Booking> bookings = await SchoolResources.GetUserBookingsAsync(kronoxReqClient);
            return Ok(bookings);
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized(new Error("Username or password incorrect."));
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while attempting to parse user bookings, please try again later."));
        }
    }

    [HttpGet("{resourceId}")]
    public async Task<ActionResult<Resource>> GetAvailabilities([FromQuery] SchoolEnum schoolId, [FromRoute] string resourceId, [FromQuery] DateTime date)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            List<Resource> resources = await SchoolResources.GetResourcesAsync(kronoxReqClient);
            Resource resource = await resources.Where(e => e.Id == resourceId).First().FetchData(kronoxReqClient, date);
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
    public async Task<ActionResult<Booking>> BookResource([FromQuery] SchoolEnum schoolId, [FromBody] BookingRequest bookingRequest)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            await SchoolResources.BookResourceAsync(kronoxReqClient, bookingRequest.ResourceId, bookingRequest.Date, bookingRequest.Slot);

            List<Booking> bookings = await SchoolResources.GetUserBookingsAsync(kronoxReqClient);
            bookings.Sort((x, y) =>
            {
                return int.Parse(x.Id[20..]) > int.Parse(y.Id[20..]) ? 0 : 1;
            });


            Booking? newBooking = bookings.Find(booking => booking.ResourceId == bookingRequest.ResourceId && booking.LocationId == bookingRequest.Slot.LocationId);

            if (newBooking == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("The requested resource was not booked for unknown reasons."));
            }

            return Accepted(newBooking);
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
    public async Task<ActionResult> UnbookResource([FromQuery] SchoolEnum schoolId, [FromQuery] string bookingId)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            await SchoolResources.UnBookResourceAsync(kronoxReqClient, bookingId);

            return Accepted();
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
    public async Task<ActionResult> ConfirmResourceBooking([FromQuery] SchoolEnum schoolId, [FromBody] ConfirmBookingRequest data)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        if (!kronoxReqClient.IsAuthenticated)
            return BadRequest(new Error("Requires provided auth token"));

        try
        {
            await SchoolResources.ConfirmResourceBookingAsync(kronoxReqClient, data.BookingId, data.ResourceId);

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
