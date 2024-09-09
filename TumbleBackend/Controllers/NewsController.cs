using DatabaseAPI.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Modules;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;
using TumbleBackend.Filters.ActionFilters;

namespace TumbleBackend.Controllers
{
    [EnableCors("CorsPolicy")]
    [ApiController]
    [Route("api/news")]
    public class NewsController : ControllerBase
    {

        private readonly ILogger<NewsController> _logger;

        public NewsController(ILogger<NewsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsHistory([FromServices] IDbNewsService newsService)
        {
            return Ok(await newsService.GetNewsHistoryAsync());
        }


        [ServiceFilter(typeof(AdminAuthFilter))]
        [HttpPost]
        public async Task<IActionResult> SendMessage([FromServices] MobileMessagingClient messaging, [FromServices] IDbNewsService newsService, [FromBody] NotificationContent notificationContent)
        {
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
}
