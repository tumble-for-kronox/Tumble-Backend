using KronoxAPI.Model.Users;
using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;

namespace TumbleBackend.Utilities;

public class TestUserUtil {
    private readonly IConfiguration _configuration;

    public TestUserUtil(IConfiguration configuration) {
        _configuration = configuration;
    }

    public bool IsTestUser(string username, string password) {
        string? testUserPass = _configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass);
        string? testUserEmail = _configuration[UserSecrets.TestUserEmail] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserEmail);
        string? testUserSessionToken = _configuration[UserSecrets.TestUserSessionToken] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserSessionToken);

        if (testUserPass == null || testUserEmail == null)
            throw new NullReferenceException("It should not be possible for testUserPass OR testUserEmail to be null at this point.");

        return username == testUserEmail && password == testUserPass;
    }
    
    public User GetTestUser() {
        string? testUserEmail = _configuration[UserSecrets.TestUserEmail] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserEmail);
        string? testUserPass = _configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass);

        if (testUserEmail == null || testUserPass == null)
            throw new NullReferenceException("Ensure that TestUserEmail and TestUserPass are defined in the environment.");

        return new User("Test User", testUserEmail, "testSessionToken");
    }
}