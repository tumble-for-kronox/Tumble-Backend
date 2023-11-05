using KronoxAPI.Model.Schools;
using KronoxAPI.Model.Users;
using Microsoft.AspNetCore.Mvc.Filters;
using TumbleBackend.StringConstants;
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

        KronoxRequestClient requestClient = (KronoxRequestClient)context.HttpContext.Items[KronoxReqClientKeys.SingleClient]!;
        requestClient.SetSessionToken(sessionToken);
        context.ActionArguments[KronoxReqClientKeys.SingleClient] = requestClient;

        base.OnActionExecuting(context);
    }
}
