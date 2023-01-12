using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Utilities;
using WebAPIModels.RequestModels;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[Route("misc")]
public class MiscController : ControllerBase
{

    [HttpPost("submitIssue")]
    public IActionResult Post([FromBody] IssueSubmission issue)
    {
        return new StatusCodeResult(statusCode: (int)EmailUtil.SendNewIssueEmail(issue.title, issue.description).Result);
    }
}
