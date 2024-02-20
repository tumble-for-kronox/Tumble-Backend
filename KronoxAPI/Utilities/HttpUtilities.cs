using System.Net.Http.Headers;
using System.Web;
using TumbleHttpClient;

namespace KronoxAPI.Utilities;

public static class HttpUtilities
{
    public static async Task<HttpResponseMessage> SendRequestAsync(IKronoxRequestClient client, HttpMethod method, string endpoint, Dictionary<string, string> parameters, string? postData = null)
    {
        var query = HttpUtility.ParseQueryString("");
        foreach (var param in parameters)
        {
            query[param.Key] = param.Value;
        }

        var fullPath = $"{endpoint}?{query}";
        HttpRequestMessage request = new(method, fullPath);

        if (postData != null)
        {
            request.Content = new StringContent(postData);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
        }

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return response;
    }
}
