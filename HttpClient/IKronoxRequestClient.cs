using System.Net;

namespace TumbleHttpClient;

public interface IKronoxRequestClient
{
    Uri? BaseUrl { get; }
    bool IsAuthenticated { get; }
    CookieContainer CookieContainer { get; }

    string? SessionToken { get; }

    Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
}
