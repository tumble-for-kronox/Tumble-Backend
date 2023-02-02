using Microsoft.AspNetCore.Mvc.Filters;

namespace TumbleBackend.ActionFilters;

public class SessionTokenActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        bool foundSessionToken = context.HttpContext.Request.Query.TryGetValue("sessionToken", out var sessionToken);

        if (!foundSessionToken)
        {
            return;
        }

        context.HttpContext.Request.Headers.Add("sessionToken", sessionToken);

        base.OnActionExecuting(context);
    }
}
