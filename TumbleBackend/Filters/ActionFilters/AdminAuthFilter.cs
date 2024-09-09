using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using TumbleBackend.Constants;

namespace TumbleBackend.Filters.ActionFilters;

public class AdminAuthFilter : ActionFilterAttribute
{
    private readonly IConfiguration _configuration;

    public AdminAuthFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string? adminPassword = (_configuration[UserSecrets.AdminPass]
            ?? Environment.GetEnvironmentVariable(EnvVar.AdminPass))
            ?? throw new NullReferenceException("Ensure that AdminPass is defined in the environment.");

        var hasAuthHeader = context.HttpContext.Request.Headers.TryGetValue("x-admin-token", out StringValues value);

        if (!hasAuthHeader || value.ToString() == string.Empty || value.ToString() == null)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.HttpContext.Response.WriteAsync("Authorization header is missing.");
            return;
        }

        string? token = value;
        if (token != adminPassword)
        {
            context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.HttpContext.Response.WriteAsync("Invalid token.");
            return;
        }

        await next();
    }
}
