using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Utilities;
using TumbleBackend.Extensions;
using WebAPIModels.ResponseModels;
using WebAPIModels.RequestModels;
using KronoxAPI.Model.Users;
using KronoxAPI.Exceptions;
using System.Text.Json;

namespace TumbleBackend.Controllers;

[ApiController]
[Route("users")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;

    public UserController(ILogger<UserController> logger)
    {
        _logger = logger;
    }

    [HttpPost("login")]
    public IActionResult LoginKronoxUser([FromQuery] SchoolEnum schoolId, [FromBody] LoginRequest body)
    {
        School? school = schoolId.GetSchool();

        if (school == null)
            return BadRequest(new Error("Invalid school value."));

        try
        {
            User kronoxUser = school.Login(body.Username, body.Password);
            return Ok(kronoxUser);
        }
        catch (LoginException e)
        {
            _logger.LogError(e.Message);
            return Unauthorized();
        }
        catch (ParseException e)
        {
            _logger.LogError(e.Message);
            return StatusCode(500);
        }
    }
}
