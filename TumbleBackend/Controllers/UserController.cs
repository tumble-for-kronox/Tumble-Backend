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
using Microsoft.AspNetCore.Cors;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetKronoxUser([FromServices] IConfiguration configuration, [FromQuery] SchoolEnum schoolId, [FromHeader(Name = "X-auth-token")] string refreshToken)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        string? jwtEncKey = configuration[UserSecrets.JwtEncryptionKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtEncryptionKey);
        string? jwtSigKey = configuration[UserSecrets.JwtSignatureKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtSignatureKey);
        string? refreshTokenExpiration = configuration[UserSecrets.JwtRefreshTokenExpiration] ?? Environment.GetEnvironmentVariable(EnvVar.JwtRefreshTokenExpiration);
        if (jwtEncKey == null || refreshTokenExpiration == null || jwtSigKey == null)
            throw new NullReferenceException("It should not be possible for jwtEncKey OR refreshTokenExpirationTime OR jwtSigKey to be null at this point.");

        RefreshTokenResponseModel? creds = JwtUtil.ValidateAndReadRefreshToken(jwtEncKey, jwtSigKey, refreshToken);

        if (creds == null)
            return Unauthorized(new Error("Couldn't login user from refreshToken, please log out and back in manually."));

        try
        {
            User kronoxUser = school.Login(creds.Username, creds.Password);

            string updatedExpirationDateRefreshToken = JwtUtil.GenerateRefreshToken(jwtEncKey, jwtSigKey, int.Parse(refreshTokenExpiration), creds.Username, creds.Password);

            return Ok(kronoxUser.ToWebModel(updatedExpirationDateRefreshToken));
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
    public IActionResult LoginKronoxUser([FromServices] IConfiguration configuration, [FromQuery] SchoolEnum schoolId, [FromBody] LoginRequest body)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            User kronoxUser = school.Login(body.Username, body.Password);

            string? jwtEncKey = configuration[UserSecrets.JwtEncryptionKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtEncryptionKey);
            string? jwtSigKey = configuration[UserSecrets.JwtSignatureKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtSignatureKey);
            string? refreshTokenExpiration = configuration[UserSecrets.JwtRefreshTokenExpiration] ?? Environment.GetEnvironmentVariable(EnvVar.JwtRefreshTokenExpiration);
            if (jwtEncKey == null || refreshTokenExpiration == null || jwtSigKey == null)
                throw new NullReferenceException("It should not be possible for jwtEncKey OR refreshTokenExpirationTime OR jwtSigKey to be null at this point.");

            string newRefreshToken = JwtUtil.GenerateRefreshToken(jwtEncKey, jwtSigKey, int.Parse(refreshTokenExpiration), body.Username, body.Password);

            return Ok(kronoxUser.ToWebModel(newRefreshToken));
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

    [HttpGet("refresh")]
    public IActionResult RefreshKronoxUser([FromServices] IConfiguration configuration, [FromQuery] SchoolEnum schoolId, [FromHeader] string authorization)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        string? jwtEncKey = configuration[UserSecrets.JwtEncryptionKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtEncryptionKey);
        string? jwtSigKey = configuration[UserSecrets.JwtSignatureKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtSignatureKey);
        string? refreshTokenExpiration = configuration[UserSecrets.JwtRefreshTokenExpiration] ?? Environment.GetEnvironmentVariable(EnvVar.JwtRefreshTokenExpiration);
        if (jwtEncKey == null || refreshTokenExpiration == null || jwtSigKey == null)
            throw new NullReferenceException("It should not be possible for jwtEncKey OR refreshTokenExpirationTime OR jwtSigKey to be null at this point.");

        RefreshTokenResponseModel? creds = JwtUtil.ValidateAndReadRefreshToken(jwtEncKey, jwtSigKey, authorization);

        if (creds == null)
            return Unauthorized(new Error("Couldn't login user from refreshToken, please log out and back in manually."));

        try
        {
            User kronoxUser = school.Login(creds.Username, creds.Password);

            string updatedExpirationDateRefreshToken = JwtUtil.GenerateRefreshToken(jwtEncKey, jwtSigKey, int.Parse(refreshTokenExpiration), creds.Username, creds.Password);

            return Ok(kronoxUser.ToWebModel(updatedExpirationDateRefreshToken));
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
