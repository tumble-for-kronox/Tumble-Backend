using TumbleBackend.StringConstants;
using TumbleBackend.Utilities;

namespace TumbleBackend.Utilities;

public class TestUserUtil {
    private readonly IConfiguration _configuration;
    private readonly string _testUserPass;
    private readonly string _testUserEmail;

    public TestUserUtil(IConfiguration configuration) {
        _configuration = configuration;
        _testUserPass = _configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass) ?? throw new NullReferenceException("Ensure that TestUserPass is defined in the environment.");
    }

    public bool IsTestUser(string username, string password) {
        string? testUserPass = _configuration[UserSecrets.TestUserPass] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserPass);
        string? testUserEmail = _configuration[UserSecrets.TestUserEmail] ?? Environment.GetEnvironmentVariable(EnvVar.TestUserEmail);

        if (testUserPass == null || testUserEmail == null)
            throw new NullReferenceException("It should not be possible for testUserPass OR testUserEmail to be null at this point.");

        return username == testUserEmail && password == testUserPass;
    }
    
}