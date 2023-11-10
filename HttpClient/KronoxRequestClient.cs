using System.Net;

namespace TumbleHttpClient;

public class KronoxRequestClient : IKronoxRequestClient
{
    private readonly HttpClient _httpClient;
    private CookieContainer _cookieContainer;
    private Uri? _baseUrl;
    private string? _sessionToken;

    public KronoxRequestClient(double timeout = 5)
    {
        _cookieContainer = new CookieContainer();
        var clientHandler = new HttpClientHandler { CookieContainer = _cookieContainer };

        var rateLimitingHandler = new RateLimitingHandler(maxRequests: 200, timeSpan: TimeSpan.FromMinutes(60))
        {
            InnerHandler = clientHandler
        };

        _httpClient = new HttpClient(rateLimitingHandler)
        {
            Timeout = TimeSpan.FromSeconds(timeout)
        };
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
    }

    public Uri? BaseUrl => _baseUrl;

    public bool IsAuthenticated => _sessionToken != null;

    public CookieContainer CookieContainer => _cookieContainer;

    public void SetSessionToken(string sessionToken)
    {
        _cookieContainer.Add(new Cookie()
        {
            Name = "JSESSIONID",
            Value = sessionToken,
            Domain = BaseUrl!.Host,
            Path = "/",
            Secure = true
        });
        _sessionToken = sessionToken;
    }

    public void SetBaseAddress(Uri baseAddress)
    {
        _httpClient.BaseAddress = baseAddress;
        _baseUrl = baseAddress;
    }

    async public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
    {
        if (_baseUrl == null)
        {
            throw new NullReferenceException("The base address of the KronoxRequestClient must be initialized before sending requests.");
        }

        return await _httpClient.SendAsync(httpRequestMessage);
    }
}