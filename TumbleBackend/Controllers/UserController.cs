using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.RequestModels;
using KronoxAPI.Exceptions;
using TumbleBackend.Utilities;
using WebAPIModels.Extensions;
using TumbleBackend.StringConstants;
using TumbleBackend.ActionFilters;
using Microsoft.AspNetCore.Cors;
using TumbleHttpClient;
using WebAPIModels.MiscModels;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[KronoxUrlFilter]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Retrieves a KronoxUser object for the user with the provided refreshToken. If the user is a test user, a test user object is returned with mock data upon further requests.
    /// </summary>
    /// <param name="jwtUtil"></param>
    /// <param name="testUserUtil"></param>
    /// <param name="schoolId"></param>
    /// <param name="refreshToken"></param>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <param name="jwtUtil"></param>
    /// <param name="testUserUtil"></param>
    /// <param name="schoolId"></param>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetKronoxUserAsync([FromServices] JwtUtil jwtUtil, [FromQuery] SchoolEnum schoolId, [FromHeader(Name = "X-auth-token")] string refreshToken)
    {
        var kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        var creds = jwtUtil.ValidateAndReadRefreshToken(refreshToken);

        if (creds == null)
            return Unauthorized(new Error("Couldn't login user from refreshToken, please log out and back in manually."));
        
        // If the user is not a test user, attempt to auto-login the user and return the KronoxUser object.
        try
        {
            var kronoxUser = await School.LoginAsync(kronoxReqClient, creds.Username, creds.Password);

            if (kronoxUser == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an unknown error while fetching user data from Kronox."));

            var updatedExpirationDateRefreshToken = jwtUtil.GenerateRefreshToken(creds.Username, creds.Password);

            return Ok(kronoxUser.ToWebModel(updatedExpirationDateRefreshToken, "", new SessionDetails(kronoxUser.SessionToken, kronoxReqClient.BaseUrl!.ToString())));
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

    /// <summary>
    /// Logs in a user with the provided username and password. If the user is a test user, a test user object is returned with mock data upon further requests.
    /// </summary>
    /// <param name="jwtUtil"></param>
    /// <param name="testUserUtil"></param>
    /// <param name="schoolId"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<IActionResult> LoginKronoxUserAsync([FromServices] JwtUtil jwtUtil, [FromQuery] SchoolEnum schoolId, [FromBody] LoginRequest body)
    {
        var kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;

        try
        {
            var kronoxUser = await School.LoginAsync(kronoxReqClient, body.Username, body.Password);

            if (kronoxUser == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an unknown error while fetching user data from Kronox."));

            var newRefreshToken = jwtUtil.GenerateRefreshToken(body.Username, body.Password);
            SessionDetails sessionDetails = new(kronoxUser.SessionToken, kronoxReqClient.BaseUrl!.ToString());

            Response.Headers.Add("X-auth-token", newRefreshToken);
            Response.Headers.Add("X-session-token", sessionDetails.ToJson());

            return Ok(kronoxUser.ToWebModel(newRefreshToken, "", sessionDetails));
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
}
