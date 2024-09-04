using DatabaseAPI;
using DatabaseAPI.Interfaces;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Utilities;
using WebAPIModels.RequestModels;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[Route("api/misc")]
public class MiscController : ControllerBase
{

    [HttpPost("submitIssue")]
    public async Task<IActionResult> SubmitIssue([FromBody] IssueSubmission issue)
    {
        return new StatusCodeResult(statusCode: (int)await EmailUtil.SendNewIssueEmail(issue.title, issue.description));
    }

    [HttpGet("news")]
    public async Task<IActionResult> GetNewsHistory([FromServices] IDbNewsService newsService)
    {
        return Ok(await newsService.GetNewsHistoryAsync());
    }
}
