using System.Net;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.ExceptionMiddleware;

public class GeneralExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GeneralExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Handle the TimeoutException here, e.g., log the exception or return a custom response.
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new Error("An unknown error occurred."));
        }
    }
}
