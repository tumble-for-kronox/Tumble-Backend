using System.Net;
using TumbleHttpClient.Exceptions;

namespace TumbleHttpClient;

public class HttpPinger
{
    private readonly HttpClient _httpClient;

    public HttpPinger(HttpClient client)
    {
        _httpClient = client;
    }

    /// <summary>
    /// Method that returns the first url of a list to return a 200 OK response.
    /// </summary>
    /// <param name="urls"></param>
    /// <returns></returns>
    /// <exception cref="NoValidUrlException"></exception>
    async public Task<Uri> Ping(string[] urls)
    {
        HttpRequestMessage[] httpRequestMessages = urls.Select(url => new HttpRequestMessage(HttpMethod.Get, url)).ToArray();

        Task<HttpResponseMessage>[] requests = httpRequestMessages.Select(url =>  _httpClient.SendAsync(url)).ToArray();

        Task<HttpResponseMessage> completedTask = await Task.WhenAny(requests);

        HttpResponseMessage response = await completedTask;

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new NoValidUrlException();
        }

        return response.RequestMessage!.RequestUri!;
    }
}
