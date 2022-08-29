using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Utilities;
using WebAPIModels.RequestModels;

namespace TumbleBackend.Controllers;

[ApiController]
[Route("misc")]
public class MiscController
{

    [HttpPost("submitIssue")]
    public IActionResult Post([FromBody] IssueSubmission issue)
    {
        return new StatusCodeResult(statusCode: (int)EmailUtil.SendNewIssueEmail(issue.title, issue.description).Result);
    }
}
