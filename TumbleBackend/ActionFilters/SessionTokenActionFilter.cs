using KronoxAPI.Model.Schools;
using KronoxAPI.Model.Users;
using Microsoft.AspNetCore.Mvc.Filters;
using TumbleHttpClient;

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

        KronoxRequestClient requestClient = (KronoxRequestClient)context.ActionArguments["kronoxReqClient"]!;
        requestClient.SetSessionToken(sessionToken);
        context.ActionArguments["kronoxReqClient"] = requestClient;

        base.OnActionExecuting(context);
    }
}
