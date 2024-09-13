using KronoxAPI.Model.Users;
using TumbleBackend.StringConstants;

namespace TumbleBackend.Utilities;

public class TestUserUtil {
    private readonly IConfiguration _configuration;
    private readonly ILogger<TestUserUtil> _logger;

    public TestUserUtil(IConfiguration configuration, ILogger<TestUserUtil> logger) {
        _logger = logger;
        _configuration = configuration;
    }

    public bool IsTestUser(string? username, string? password) {
        if (username == null || password == null)
        {
            _logger.LogError("Username or password was null.");
            return false;
        }
        
        string? testUserPass = _configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass);
        string? testUserEmail = _configuration[UserSecrets.TestUserEmail] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserEmail);

        if (testUserPass == null || testUserEmail == null)
            throw new NullReferenceException("Test user credentials are null. Ensure that TestUserEmail and TestUserPass are defined");

        return username == testUserEmail && password == testUserPass;
    }
    
    public User GetTestUser() {
        string? testUserEmail = _configuration[UserSecrets.TestUserEmail] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserEmail);
        string? testUserPass = _configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass);

        if (testUserEmail == null || testUserPass == null)
            throw new NullReferenceException("Test user credentials are null. Ensure that TestUserEmail and TestUserPass are defined");

        return new User("Test User", testUserEmail, "testSessionToken");
    }
}