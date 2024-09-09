using KronoxAPI.Exceptions;
using KronoxAPI.Model.Schools;
using KronoxAPI.Model.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text.RegularExpressions;
using KronoxBackend.Extensions;
using KronoxBackend.InternalModels;
using KronoxBackend.StringConstants;
using KronoxBackend.Utilities;
using TumbleHttpClient;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;

namespace KronoxBackend.ActionFilters;

public class AuthActionFilter : ActionFilterAttribute
{
    private StringValues refreshToken;
    private StringValues schoolIdQuery;
    private SessionDetails? sessionDetails;
    private readonly IConfiguration configuration;

    public AuthActionFilter(IConfiguration config)
    {
        configuration = config;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var services = context.HttpContext.RequestServices;
        var jwtUtil = services.GetService<JwtUtil>()!;

        bool hasAuthHeader = context.HttpContext.Request.Headers.TryGetValue("X-auth-token", out refreshToken);
        bool hasSessionDetails = context.HttpContext.Request.Headers.TryGetValue("X-session-token", out var sessionDetailsJson);

        if (hasSessionDetails)
        {
            sessionDetails = SessionDetails.FromJson(sessionDetailsJson);
        }

        if (!hasAuthHeader)
        {
            await next();
            return;
        }

        bool foundSchoolId = context.HttpContext.Request.Query.TryGetValue("schoolId", out schoolIdQuery);

        if (!foundSchoolId)
        {
            context.Result = new BadRequestObjectResult(new Error("Requires schoolId query parameter."));
            return;
        }

        SchoolEnum schoolId = (SchoolEnum)int.Parse(schoolIdQuery);

        School? school = schoolId.GetSchool();

        if (school == null)
        {
            context.Result = new BadRequestObjectResult(new Error("Invalid school value."));
            return;
        }

        RefreshTokenResponseModel? creds = jwtUtil.ValidateAndReadRefreshToken(refreshToken);

        if (creds == null)
        {
            context.Result = new UnauthorizedObjectResult(new Error("Couldn't login user from refreshToken, please log out and back in manually."));
            return;
        }
        string newRefreshToken = jwtUtil.GenerateRefreshToken(creds.Username, creds.Password);

        KronoxRequestClient requestClient = (KronoxRequestClient)context.HttpContext.Items[KronoxReqClientKeys.SingleClient]!;

        try
        {

            if (sessionDetails != null)
            {
                requestClient.SetSessionToken(sessionDetails.SessionToken);

                bool sessionValid = await School.RefreshUserSessionAsync(requestClient);
                if (sessionValid)
                {
                    context.HttpContext.Items[KronoxReqClientKeys.SingleClient] = requestClient;

                    context.HttpContext.Response.Headers.Add("X-auth-token", newRefreshToken);

                    await Next(context, requestClient, next);
                    return;
                }
            }

            User? kronoxUser = await School.LoginAsync(requestClient, creds.Username, creds.Password);

            if (kronoxUser == null)
            {
                context.Result = new ObjectResult(new Error("There was an unknown error while fetching user data from Kronox."))
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError
                };
                return;
            }

            context.HttpContext.Response.Headers.Add("X-auth-token", newRefreshToken);

            requestClient.SetSessionToken(kronoxUser.SessionToken);
            context.HttpContext.Items[KronoxReqClientKeys.SingleClient] = requestClient;
        }
        catch (LoginException e)
        {
            context.Result = new UnauthorizedObjectResult(new Error("Username or password incorrect."));
            return;
        }
        catch (ParseException)
        {
            context.Result = new ObjectResult(StatusCodes.Status500InternalServerError);
            return;
        }

        await Next(context, requestClient, next);
    }

    private async Task Next(ActionExecutingContext context, KronoxRequestClient requestClient, ActionExecutionDelegate next)
    {
        var finishedContext = await next();
        if (requestClient.SessionToken == null || requestClient.BaseUrl == null || finishedContext.Result is not OkObjectResult)
        {
            return;
        }

        SessionDetails refreshedSessionDetails = new(requestClient.SessionToken, requestClient.BaseUrl.ToString());
        var jsonRefreshedSessionDetails = Regex.Unescape(refreshedSessionDetails.ToJson());
        context.HttpContext.Response.Headers.Add("X-session-token", jsonRefreshedSessionDetails);
    }
}
