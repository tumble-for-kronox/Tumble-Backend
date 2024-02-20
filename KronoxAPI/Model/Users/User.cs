namespace KronoxAPI.Model.Users;

/// <summary>
/// Model for fetching and managing user data in Kronox's database. 
/// </summary>
public class User
{
    public User(string name, string username, string sessionToken)
    {
        Name = name;
        Username = username;
        SessionToken = sessionToken;
    }

    public string Name { get; }

    public string Username { get; }

    public string SessionToken { get; set; }

    /// <summary>
    /// For use as default or in case a user is not found.
    /// </summary>
    /// <returns><see cref="User"/> with all values set as "N/A".</returns>
    public static User NotAvailable => new("N/A", "N/A", "N/A");

}
