using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using TumbleBackend.Library;
using WebAPIModels.MiscModels;
using System.Net;
using System.Web.Http.Results;
using WebAPIModels.ResponseModels;
using DatabaseAPI;
using TumbleBackend.StringConstants;
using DatabaseAPI.Interfaces;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[Route("api/admin")]
public class AdminController : Controller
{
    [HttpPut("notification")]
    public async Task<IActionResult> SendMessage([FromServices] MobileMessagingClient messaging, [FromServices] IConfiguration configuration, [FromServices] IDbNewsService newsService, [FromHeader] string auth, [FromQuery] NotificationContent notificationContent)
    {
        string? adminPassword = configuration[UserSecrets.AdminPass] ?? Environment.GetEnvironmentVariable(EnvVar.AdminPass);

        if (adminPassword == null)
            throw new NullReferenceException("Ensure that AdminPass is defined in the environment.");

        if (auth != adminPassword)
        {
            return Unauthorized();
        }

        string result_id = await messaging.SendNotification(notificationContent.Topic, notificationContent.Title, notificationContent.Body);

        if (result_id != string.Empty)
        {
            await newsService.SaveNewsItemAsync(notificationContent);
            return Ok();
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while creating the notification."));
        }
    }
}
