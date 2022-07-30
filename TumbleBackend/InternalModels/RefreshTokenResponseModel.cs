namespace TumbleBackend.InternalModels;

public class RefreshTokenResponseModel
{
    public string Username { get; internal set; }
    public string Password { get; internal set; }

    public RefreshTokenResponseModel(string username, string password)
    {
        Username = username;
        Password = password;
    }
}
