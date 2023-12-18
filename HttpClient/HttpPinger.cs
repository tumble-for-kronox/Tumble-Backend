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
    /// Pings a list of URLs and returns the first URL to correctly return a 200 http code.
    /// Optionally prioritizes a given URL and returns it immediately if it pings successfully.
    /// If the prioritized URL fails, returns the first successful URL from the others.
    /// If none are successful, throws <see cref="NoValidUrlException"/>.
    /// </summary>
    /// <param name="urls">The list of URLs to ping.</param>
    /// <param name="priorityUrl">The optional URL to prioritize.</param>
    /// <returns>The first successful URL, or the prioritized URL if successful.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="NoValidUrlException"></exception>
    public async Task<Uri> PingAsync(IEnumerable<string> urls, string? priorityUrl = null)
    {
        if (urls == null)
            throw new ArgumentNullException(nameof(urls));

        var tasks = urls.Select(url => PingUrlAsync(url)).ToList();
        Task<HttpResponseMessage>? priorityTask = null;

        if (!string.IsNullOrEmpty(priorityUrl) && urls.Contains(priorityUrl))
        {
            // Create a separate task for the priority URL
            priorityTask = PingUrlAsync(priorityUrl);

            // Wait for the priority URL to complete
            var priorityResponse = await priorityTask;
            if (priorityResponse.StatusCode == HttpStatusCode.OK)
            {
                return new Uri(priorityUrl);
            }
        }

        // Continue with other URLs if priority URL is not successful
        while (tasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(tasks);

            try
            {
                var response = await completedTask;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.RequestMessage!.RequestUri!;
                }
            }
            catch
            {
                // Exceptions are ignored as we only care about successful pings
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
