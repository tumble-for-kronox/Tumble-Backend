using System.Text.Json;

namespace WebAPIModels.ResponseModels;

public class Error
{
    private readonly string _message;

    public string Message => _message;

    public Error(string message)
    {
        _message = message;
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
