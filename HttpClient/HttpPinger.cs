using System.Net;
using System.Net.Http;
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
    /// Takes a list of URLs as strings and returns the first URL to correctly return a 200 http code. If none do, throws <see cref="NoValidUrlException"/>.
    /// </summary>
    /// <param name="urls"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NoValidUrlException"></exception>
    public async Task<Uri> PingAsync(IEnumerable<string> urls)
    {
        if (urls == null)
            throw new ArgumentNullException(nameof(urls));

        List<Task<HttpResponseMessage>> tasks = urls.Select(url => PingUrlAsync(url)).ToList();

        while (tasks.Count > 0)
        {
            Task<HttpResponseMessage> completedTask = await Task.WhenAny(tasks);

            try
            {
                HttpResponseMessage response = await completedTask;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.RequestMessage!.RequestUri!;
                }
            }
            catch (Exception)
            {
                // Exceptions do not require handling here, we just care if the URLs work or not
            }

            tasks.Remove(completedTask);
        }

        throw new NoValidUrlException("None of the URLs could be fetched with a 200 OK status code.");
    }

    private async Task<HttpResponseMessage> PingUrlAsync(string url)
    {
        try
        {
            return await _httpClient.GetAsync(url);
        }
        catch
        {
            return new HttpResponseMessage(HttpStatusCode.InternalServerError);
        }
    }
}
