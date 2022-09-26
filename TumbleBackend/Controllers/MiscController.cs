using Microsoft.AspNetCore.Mvc;
using TumbleBackend.Utilities;
using WebAPIModels.RequestModels;
using System.Web.Http.Cors;

namespace TumbleBackend.Controllers;

[EnableCors(origins: "*", headers: "*", methods: "*")]
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
