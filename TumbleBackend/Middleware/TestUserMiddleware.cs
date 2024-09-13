using System.Text.Json;
using KronoxAPI.Model.Booking;
using TumbleBackend.Utilities;
using WebAPIModels.Extensions;
using WebAPIModels.MiscModels;
using WebAPIModels.ResponseModels;

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

        _logger.LogInformation("HTTP path: " + path);
        
        if (IsLoginRequest(context) && TryGetCredentials(body, out var username, out var password))
        {
            if (testUserUtil.IsTestUser(username, password))
            {
                _logger.LogInformation("Handling test user login response.");
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
                    _logger.LogInformation("Handling test user booking routes.");
                    await HandleTestUserBookingRoutes(context, path);
                    return;
                }

                if (path.StartsWithSegments("/api/users")) {
                    _logger.LogInformation("Handling test user routes.");
                    await HandleTestUserRoutes(context, jwtUtil, testUserUtil, path, creds.Username, creds.Password);
                    return;
                }

                await HandleTestUserTokenResponse(context, testUserUtil, refreshToken);
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

    private async Task HandleTestUserRoutes(HttpContext context, JwtUtil jwtUtil, TestUserUtil testUserUtil, PathString path, string username, string password)
    {
        if (context.Request.Method == HttpMethods.Get && path == "/api/users/events/") {
            _logger.LogInformation("Returning mock user events.");
            await ReturnMockData<UserEventCollection>(context, "mockUserEvents.json");
        }
        else if (context.Request.Method == HttpMethods.Get && path == "/api/users")
        {
            _logger.LogInformation("Returning test user information.");
            await HandleTestUserInfoResponse(context, jwtUtil, testUserUtil, username, password);
        }
    }

    private static bool IsLoginRequest(HttpContext context)
    {
        return context.Request.Method == HttpMethods.Post && context.Request.Path == "/api/users/login";
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

    private static async Task HandleTestUserTokenResponse(HttpContext context, TestUserUtil testUserUtil, string refreshToken)
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
        _logger.LogInformation("Mock data path is: " + _mockDataPath);
        if (context.Request.Method == HttpMethods.Get && path == "/api/resources")
        {
            _logger.LogInformation("Returning mock resources.");
            await ReturnMockData<List<Resource>>(context, "mockResources.json");
        }
        else if (context.Request.Method == HttpMethods.Get && path == "/api/resources/all")
        {
            _logger.LogInformation("Returning mock resources with availabilities.");
            await ReturnMockData<List<Resource>>(context, "mockResourcesWithAvailabilities.json");
        }
        else if (context.Request.Method == HttpMethods.Get && path == "/api/users/events") {
            _logger.LogInformation("Returning mock user events.");
            await ReturnMockData<UserEventCollection>(context, "mockUserEvents.json");
        }
        else if (context.Request.Method == HttpMethods.Get && path == "/api/resources/userbookings")
        {
            _logger.LogInformation("Returning mock user bookings.");
            await ReturnMockData<List<Booking>>(context, "mockUserBookings.json");
        }
        else if (context.Request.Method == HttpMethods.Put && path.StartsWithSegments("/api/resources/book"))
        {
            _logger.LogInformation("Returning mock booking result.");
            await ReturnMockData<object>(context, "mockBookingResult.json");
        }
        else if (context.Request.Method == HttpMethods.Put && path.StartsWithSegments("/api/resources/unbook"))
        {
            _logger.LogInformation("Returning mock unbooking result.");
            await ReturnMockData<object>(context, "mockUnbookingResult.json");
        }
        else if (context.Request.Method == HttpMethods.Put && path.StartsWithSegments("/api/resources/confirm"))
        {
            _logger.LogInformation("Returning mock booking confirmation.");
            await ReturnMockData<object>(context, "mockBookingConfirmation.json");
        }
    }

    private async Task ReturnMockData<T>(HttpContext context, string fileName)
    {
        var mockFilePath = Path.Combine(_mockDataPath, fileName);
        _logger.LogInformation("Retrieving mock data from path: " + mockFilePath);
        var mockData = await File.ReadAllTextAsync(mockFilePath);
        var data = JsonSerializer.Deserialize<T>(mockData);

        context.Response.StatusCode = StatusCodes.Status200OK;
        await context.Response.WriteAsJsonAsync(data);
    }
}