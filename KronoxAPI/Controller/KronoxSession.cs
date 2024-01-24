using System.Web;
using TumbleHttpClient;

namespace KronoxAPI.Controller;

public static class KronoxSession
{
    public static async Task SetSessionEnglish(IKronoxRequestClient client)
    {
        string langUri = "ajax/ajax_lang.jsp?lang=EN";

        // Perform web request for language change
        using var langRequest = new HttpRequestMessage(new HttpMethod("GET"), langUri);
        await client.SendAsync(langRequest);
    }

    public static async Task<bool> RefreshSession(IKronoxRequestClient client)
    {
        string refreshUri = "ajax/ajax_session.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "poll";

        string fullPath = $"{refreshUri}?{query}";
        using var refreshSessionRequest = new HttpRequestMessage(new HttpMethod("POST"), fullPath);
        var response = await client.SendAsync(refreshSessionRequest);

        var body = await response.Content.ReadAsStringAsync();

        return body == "OK";
    }
}
