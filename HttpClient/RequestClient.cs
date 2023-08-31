namespace TumbleHttpClient;

public class RequestClient
{
    private readonly HttpClient _httpClient;

    public RequestClient(HttpClient client, string baseUrl, string sessionToken, double timeout = 5)
    {
        client.Timeout = TimeSpan.FromSeconds(timeout);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
        client.BaseAddress = new Uri(baseUrl);
        this._httpClient = client;
    }

    async public Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage)
    {
        return await _httpClient.SendAsync(httpRequestMessage);
    }
}