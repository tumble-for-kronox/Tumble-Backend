using System.Text.Json;

namespace WebAPIModels.MiscModels;

public class SessionDetails
{
    public SessionDetails(string sessionToken, string sessionLocation)
    {
        SessionToken = sessionToken;
        SessionLocation = sessionLocation;
    }

    public string SessionToken { get; private set; }
    public string SessionLocation { get; private set; }

    public static SessionDetails? FromJson(string value)
    {
        return JsonSerializer.Deserialize<SessionDetails>(value, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }).ToString();
}