using System.Text.Json;
using KronoxAPI.Model.Booking;
using TumbleBackend.Utilities;
using WebAPIModels.Extensions;
using WebAPIModels.MiscModels;

namespace TumbleBackend.Middleware;

public class TestUserMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _mockDataPath;
    private readonly ILogger<TestUserMiddleware> _logger;

    public TestUserMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<TestUserMiddleware> logger)
    {
        _next = next;
        _mockDataPath = Path.Combine(env.ContentRootPath, "MockData");
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, JwtUtil jwtUtil, TestUserUtil testUserUtil)
    {
        var refreshToken = context.Request.Headers["X-auth-token"].FirstOrDefault();
        var path = context.Request.Path;

        context.Request.EnableBuffering();
        var body = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;
        
        if (IsLoginRequest(context) && TryGetCredentials(body, out var username, out var password))
        {
            if (testUserUtil.IsTestUser(username, password))
            {
                await HandleTestUserLoginResponse(context, jwtUtil, testUserUtil, username, password);
                return;
            }
        }
        else if (refreshToken != null)
        {
            var creds = jwtUtil.ValidateAndReadRefreshToken(refreshToken);
            
            if (creds != null && testUserUtil.IsTestUser(creds.Username, creds.Password))
            {
                if (path.StartsWithSegments("/api/resources"))
                {
                    await HandleTestUserBookingRoutes(context, path);
                    return;
                }

                if (path.StartsWithSegments("/api/users"))
                {
                    _logger.LogInformation("Returning test user information.");
                    await HandleTestUserInfoResponse(context, jwtUtil, testUserUtil, creds.Username, creds.Password);
                    return;
                }

                await HandleTestUserTokenResponse(context, jwtUtil, testUserUtil, creds.Username, creds.Password, refreshToken);
                return;
            }
        }

        _logger.LogInformation("TestUserMiddleware: No test user found. Proceeding to next middleware.");
        await _next(context);
    }


    private static async Task HandleTestUserInfoResponse(HttpContext context, JwtUtil jwtUtil, TestUserUtil testUserUtil, string username, string password)
    {
        var testUser = testUserUtil.GetTestUser();
        var newRefreshToken = jwtUtil.GenerateRefreshToken(username, password);
        var sessionDetails = new SessionDetails("", "");

        context.Response.Headers.Add("X-auth-token", newRefreshToken);
        context.Response.Headers.Add("X-session-token", sessionDetails.ToJson());

        var testUserModel = testUser.ToWebModel(newRefreshToken, "", sessionDetails);
        await context.Response.WriteAsJsonAsync(testUserModel);
    }


    private static bool IsLoginRequest(HttpContext context)
    {
        return context.Request.Method == HttpMethods.Post && context.Request.Path == "/api/users/login";
    }

    private static bool IsGetUserRequest(HttpContext context)
    {
        return context.Request.Method == HttpMethods.Get && context.Request.Path == "/api/users";
    }

    private static bool TryGetCredentials(string body, out string? username, out string? password)
    {
        username = null;
        password = null;

        var json = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
        if (json != null && json.ContainsKey("username") && json.ContainsKey("password"))
        {
            username = json["username"];
            password = json["password"];
            return true;
        }

        return false;
    }

    private static async Task HandleTestUserLoginResponse(HttpContext context, JwtUtil jwtUtil, TestUserUtil testUserUtil, string? username, string? password)
    {
        if (username == null || password == null)
            throw new ArgumentNullException("Username and password must be provided.");

        var testUser = testUserUtil.GetTestUser();
        var newRefreshToken = jwtUtil.GenerateRefreshToken(username, password);
        var sessionDetails = new SessionDetails("", "");

        context.Response.Headers.Add("X-auth-token", newRefreshToken);
        context.Response.Headers.Add("X-session-token", sessionDetails.ToJson());

        var testUserModel = testUser.ToWebModel(newRefreshToken, "", sessionDetails);
        await context.Response.WriteAsJsonAsync(testUserModel);
    }

    private static async Task HandleTestUserTokenResponse(HttpContext context, JwtUtil jwtUtil, TestUserUtil testUserUtil, string username, string password, string refreshToken)
    {
        var testUser = testUserUtil.GetTestUser();
        var sessionDetails = new SessionDetails("", "");

        context.Response.Headers.Add("X-auth-token", refreshToken);
        context.Response.Headers.Add("X-session-token", sessionDetails.ToJson());

        var testUserModel = testUser.ToWebModel(refreshToken, "", sessionDetails);
        await context.Response.WriteAsJsonAsync(testUserModel);
    }

    private async Task HandleTestUserBookingRoutes(HttpContext context, PathString path)
    {
        if (context.Request.Method == HttpMethods.Get && path == "/api/resources")
        {
            await ReturnMockResources(context);
        }
        else if (context.Request.Method == HttpMethods.Get && path == "/api/resources/all")
        {
            await ReturnMockResourcesWithAvailabilities(context);
        }
        else if (context.Request.Method == HttpMethods.Get && path == "/api/resources/userbookings")
        {
            await ReturnMockUserBookings(context);
        }
        else if (context.Request.Method == HttpMethods.Put && path.StartsWithSegments("/api/resources/book"))
        {
            await ReturnMockBookingResult(context);
        }
        else if (context.Request.Method == HttpMethods.Put && path.StartsWithSegments("/api/resources/unbook"))
        {
            await ReturnMockUnbookingResult(context);
        }
        else if (context.Request.Method == HttpMethods.Put && path.StartsWithSegments("/api/resources/confirm"))
        {
            await ReturnMockBookingConfirmation(context);
        }
    }

    private async Task ReturnMockResources(HttpContext context)
    {
        var mockFilePath = Path.Combine(_mockDataPath, "mockResources.json");
        var mockResources = await File.ReadAllTextAsync(mockFilePath);
        var resources = JsonSerializer.Deserialize<List<Resource>>(mockResources);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(resources);
    }

    private async Task ReturnMockResourcesWithAvailabilities(HttpContext context)
    {
        var mockFilePath = Path.Combine(_mockDataPath, "mockResourcesWithAvailabilities.json");
        var mockResources = await File.ReadAllTextAsync(mockFilePath);
        var resources = JsonSerializer.Deserialize<List<Resource>>(mockResources);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(resources);
    }

    private async Task ReturnMockUserBookings(HttpContext context)
    {
        var mockFilePath = Path.Combine(_mockDataPath, "mockUserBookings.json");
        var mockBookings = await File.ReadAllTextAsync(mockFilePath);
        var bookings = JsonSerializer.Deserialize<List<Booking>>(mockBookings);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(bookings);
    }

    private async Task ReturnMockBookingResult(HttpContext context)
    {
        var mockFilePath = Path.Combine(_mockDataPath, "mockBookingResult.json");
        var mockResult = await File.ReadAllTextAsync(mockFilePath);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(JsonSerializer.Deserialize<object>(mockResult));
    }

    private async Task ReturnMockUnbookingResult(HttpContext context)
    {
        var mockFilePath = Path.Combine(_mockDataPath, "mockUnbookingResult.json");
        var mockResult = await File.ReadAllTextAsync(mockFilePath);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(JsonSerializer.Deserialize<object>(mockResult));
    }

    private async Task ReturnMockBookingConfirmation(HttpContext context)
    {
        var mockFilePath = Path.Combine(_mockDataPath, "mockBookingConfirmation.json");
        var mockResult = await File.ReadAllTextAsync(mockFilePath);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(JsonSerializer.Deserialize<object>(mockResult));
    }
}
