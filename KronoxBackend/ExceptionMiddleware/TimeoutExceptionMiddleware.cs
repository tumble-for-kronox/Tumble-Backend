using System.Net;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.ExceptionMiddleware;

public class TimeoutExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public TimeoutExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (TimeoutException)
        {
            // Handle the TimeoutException here, e.g., log the exception or return a custom response.
            context.Response.StatusCode = (int)HttpStatusCode.GatewayTimeout;
            await context.Response.WriteAsJsonAsync(new Error("Connection to Kronox timed out."));
        }
    }
}
