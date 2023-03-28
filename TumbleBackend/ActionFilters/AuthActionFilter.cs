using KronoxAPI.Exceptions;
using KronoxAPI.Model.Schools;
using KronoxAPI.Model.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TumbleBackend.Extensions;
using TumbleBackend.InternalModels;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;
using WebAPIModels.RequestModels;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.ActionFilters;

public class AuthActionFilter : ActionFilterAttribute
{
    private StringValues refreshToken;
    private StringValues schoolIdQuery;
    private readonly IConfiguration configuration;

    public AuthActionFilter(IConfiguration config)
    {
        this.configuration = config;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        bool foundSchoolId = context.HttpContext.Request.Query.TryGetValue("schoolId", out schoolIdQuery);

        if (!foundSchoolId)
        {
            context.Result = new BadRequestObjectResult(new Error("Requires schoolId query parameter."));
            await next();
            return;
        }

        SchoolEnum schoolId = (SchoolEnum)int.Parse(schoolIdQuery);

        bool hasAuthHeader = context.HttpContext.Request.Headers.TryGetValue("X-auth-token", out refreshToken);

        if (!hasAuthHeader)
        {
            await next();
            return;
        }

        School? school = schoolId.GetSchool();

        if (school == null)
        {
            context.Result = new BadRequestObjectResult(new Error("Invalid school value."));
            await next();
            return;
        }

        string? jwtEncKey = configuration[UserSecrets.JwtEncryptionKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtEncryptionKey);
        string? jwtSigKey = configuration[UserSecrets.JwtSignatureKey] ?? Environment.GetEnvironmentVariable(EnvVar.JwtSignatureKey);
        string? refreshTokenExpiration = configuration[UserSecrets.JwtRefreshTokenExpiration] ?? Environment.GetEnvironmentVariable(EnvVar.JwtRefreshTokenExpiration);
        if (jwtEncKey == null || refreshTokenExpiration == null || jwtSigKey == null)
            throw new NullReferenceException("It should not be possible for jwtEncKey OR refreshTokenExpirationTime OR jwtSigKey to be null at this point.");

        RefreshTokenResponseModel? creds = JwtUtil.ValidateAndReadRefreshToken(jwtEncKey, jwtSigKey, refreshToken);

        if (creds == null)
        {
            context.Result = new UnauthorizedObjectResult(new Error("Couldn't login user from refreshToken, please log out and back in manually."));
            await next();
            return;
        }

        try
        {
            User kronoxUser = await school.Login(creds.Username, creds.Password);

            //string updatedExpirationDateRefreshToken = JwtUtil.GenerateRefreshToken(jwtEncKey, jwtSigKey, int.Parse(refreshTokenExpiration), creds.Username, creds.Password);

            context.HttpContext.Request.Headers.Add("sessionToken", kronoxUser.SessionToken);
            await next();
        }
        catch (LoginException e)
        {
            context.Result = new UnauthorizedObjectResult(new Error("Username or password incorrect."));
            await next();
            return;
        }
        catch (ParseException e)
        {
            context.Result = new ObjectResult(StatusCodes.Status500InternalServerError);
            await next();
            return;
        }
    }
}
