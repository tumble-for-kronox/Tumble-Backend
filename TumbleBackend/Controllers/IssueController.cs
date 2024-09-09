using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Modules;
using WebAPIModels.RequestModels;

namespace TumbleBackend.Controllers;

[EnableCors("CorsPolicy")]
[ApiController]
[Route("api/misc")]
public class IssueController : ControllerBase
{

    [HttpPost("submitIssue")]
    public async Task<IActionResult> SubmitIssue([FromBody] IssueSubmission issue)
    {
        return new StatusCodeResult(statusCode: (int)await EmailUtil.SendNewIssueEmail(issue.title, issue.description));
    }

}
