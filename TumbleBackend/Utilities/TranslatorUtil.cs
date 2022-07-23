using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebAPIModels;
using TumbleBackend.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace TumbleBackend.Utilities;

public static class TranslatorUtil
{
    private static string? subscriptionKey = null;
    private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com/";
    private static readonly string location = "swedencentral";

    public static void Init(string translatorKey)
    {
        subscriptionKey = translatorKey;
    }

    public static async Task<string> SwedishToEnglish(string s)
    {
        if (subscriptionKey == null)
            throw new TranslatorUninitializedException("Translation utility was not initialized before being accessed.");

        string route = "/translate?api-version=3.0&from=sv&to=en";
        object[] body = new object[] { new { Text = s } };
        var requestBody = System.Text.Json.JsonSerializer.Serialize(body);

        using var client = new HttpClient();
        using var request = new HttpRequestMessage();
        // Build the request.
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(endpoint + route);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
        request.Headers.Add("Ocp-Apim-Subscription-Region", location);

        // Send the request and get response.
        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
        // Read response as a string.
        string responseString = response.Content.ReadAsStringAsync().Result;

        JObject jsonObject = JsonConvert.DeserializeObject<JObject>(responseString.TrimEnd(']').TrimStart('['))!;
        JToken token = jsonObject.SelectToken("$.translations[:1].text")!;

        return token.Value<string>()!;
    }
}
