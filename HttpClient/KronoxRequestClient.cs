using System.Diagnostics;
using System.Net;

namespace TumbleHttpClient;

public class KronoxRequestClient : IKronoxRequestClient
{
    private readonly HttpClient _httpClient;

    public KronoxRequestClient(double timeout = 5)
    {
        CookieContainer = new CookieContainer();
        HttpClientHandler handler = new()
        {
            CookieContainer = CookieContainer
        };
        _httpClient = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(timeout)
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
    }

    public Uri? BaseUrl { get; private set; }

    public bool IsAuthenticated => SessionToken != null;

    public string? SessionToken { get; private set; }

    public CookieContainer CookieContainer { get; }

    public void SetSessionToken(string sessionToken)
    {
        CookieContainer.Add(new Cookie()
        {
            Name = "JSESSIONID",
            Value = sessionToken,
            Domain = BaseUrl!.Host,
            Path = "/",
            Secure = true
        });
        SessionToken = sessionToken;
    }

    public void SetBaseAddress(Uri baseAddress)
    {
        _httpClient.BaseAddress = baseAddress;
        BaseUrl = baseAddress;
    }

    public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
    {
        if (BaseUrl == null)
        {
            throw new NullReferenceException("The base address of the KronoxRequestClient must be initialized before sending requests.");
        }

        return await _httpClient.SendAsync(httpRequestMessage);
    }
}