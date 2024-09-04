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
        string route = "https://libretranslate.com/translate";
        object[] body = new object[] { new { q = s, source = "sv", target = "en", format = "text" } };
        var requestBody = System.Text.Json.JsonSerializer.Serialize(body);

        using var client = new HttpClient();
        using var request = new HttpRequestMessage();
        // Build the request.
        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(endpoint + route);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        // Send the request and get response.
        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);
        // Read response as a string.
        string responseString = response.Content.ReadAsStringAsync().Result;

        JObject jsonObject = JsonConvert.DeserializeObject<JObject>(responseString.TrimEnd(']').TrimStart('['))!;
        JToken token = jsonObject.SelectToken("$.translatedText")!;

        return token.Value<string>()!;
    }
}
