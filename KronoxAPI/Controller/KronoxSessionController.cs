namespace KronoxAPI.Controller;

using TumbleHttpClient;
using Utilities;

public static class KronoxSessionController
{
    public static async Task SetSessionEnglishAsync(IKronoxRequestClient client)
    {
        const string endpoint = "ajax/ajax_lang.jsp";
        var parameters = new Dictionary<string, string>
        {
            ["lang"] = "EN"
        };

        await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
    }

    public static async Task<bool> RefreshSessionAsync(IKronoxRequestClient client)
    {
        const string endpoint = "ajax/ajax_session.jsp";
        var parameters = new Dictionary<string, string>
        {
            ["op"] = "poll"
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Post, endpoint, parameters, "");
        var content = await response.Content.ReadAsStringAsync();

        return content.Trim().Equals("OK", StringComparison.OrdinalIgnoreCase);
    }
}

