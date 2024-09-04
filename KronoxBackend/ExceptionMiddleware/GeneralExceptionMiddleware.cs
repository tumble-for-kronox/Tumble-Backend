using Newtonsoft.Json;
using System.Net;
using WebAPIModels.ResponseModels;

namespace TumbleBackend.ExceptionMiddleware;

public class GeneralExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GeneralExceptionMiddleware> _logger;

    public GeneralExceptionMiddleware(RequestDelegate next, ILogger<GeneralExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // Here you can add more detailed error info if needed
        var result = JsonConvert.SerializeObject(new Error("An unknown error occured while processing your request."));
        return context.Response.WriteAsync(result);
    }

}
