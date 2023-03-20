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

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[Route("api/admin")]
public class AdminController : Controller
{
    [HttpPut("notification")]
    public async Task<IActionResult> SendMessage([FromServices] MobileMessagingClient messaging, [FromQuery] NotificationContent notificationContent)
    {
        string result_id = await messaging.SendNotification(notificationContent.Topic, notificationContent.Title, notificationContent.Body);

        if (result_id != string.Empty)
        {
            NewsHistory.SaveNewsItem(notificationContent);
            return Ok();
        }
        else
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new Error("An error occurred while creating the notification."));
        }
    }
}
