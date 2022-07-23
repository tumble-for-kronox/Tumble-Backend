using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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
