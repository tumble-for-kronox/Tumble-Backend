using TumbleHttpClient;

namespace KronoxAPI.Controller;

public static class KronoxEnglishSession
{
    public static async Task SetSessionEnglish(IKronoxRequestClient client)
    {
        string langUri = "ajax/ajax_lang.jsp?lang=EN";

        // Perform web request for language change
        using var langRequest = new HttpRequestMessage(new HttpMethod("GET"), langUri);
        await client.SendAsync(langRequest);
    }

}
