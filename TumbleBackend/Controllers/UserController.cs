using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.RequestModels;
using KronoxAPI.Model.Users;
using KronoxAPI.Exceptions;
using TumbleBackend.Utilities;
using WebAPIModels.Extensions;
using TumbleBackend.InternalModels;
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

    [HttpGet]
    public async Task<IActionResult> GetKronoxUser([FromServices] JwtUtil jwtUtil, [FromQuery] SchoolEnum schoolId, [FromHeader(Name = "X-auth-token")] string refreshToken)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        RefreshTokenResponseModel? creds = jwtUtil.ValidateAndReadRefreshToken(refreshToken);

        if (creds == null)
            return Unauthorized(new Error("Couldn't login user from refreshToken, please log out and back in manually."));

        try
        {
            User? kronoxUser = await School.LoginAsync(kronoxReqClient, creds.Username, creds.Password);

            if (kronoxUser == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an unknown error while fetching user data from Kronox."));

            string updatedExpirationDateRefreshToken = jwtUtil.GenerateRefreshToken(creds.Username, creds.Password);

            return Ok(kronoxUser.ToWebModel(updatedExpirationDateRefreshToken, new(kronoxUser.SessionToken, kronoxReqClient.BaseUrl!.ToString())));
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

    [HttpPost("login")]
    public async Task<IActionResult> LoginKronoxUser([FromServices] JwtUtil jwtUtil, [FromQuery] SchoolEnum schoolId, [FromBody] LoginRequest body)
    {
        IKronoxRequestClient kronoxReqClient = (IKronoxRequestClient)HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        School school = schoolId.GetSchool()!;

        try
        {
            User? kronoxUser = await School.LoginAsync(kronoxReqClient, body.Username, body.Password);

            if (kronoxUser == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Error("There was an unknown error while fetching user data from Kronox."));

            string newRefreshToken = jwtUtil.GenerateRefreshToken(body.Username, body.Password);

            SessionDetails sessionDetails = new(kronoxUser.SessionToken, kronoxReqClient.BaseUrl!.ToString());

            Response.Headers.Add("X-auth-token", newRefreshToken);
            Response.Headers.Add("X-session-token", sessionDetails.ToJson());
            return Ok(kronoxUser.ToWebModel(newRefreshToken, sessionDetails));
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
