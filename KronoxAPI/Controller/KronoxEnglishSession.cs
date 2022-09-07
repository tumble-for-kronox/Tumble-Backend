using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Controller;

public static class KronoxEnglishSession
{
    static readonly HttpClientHandler clientHandler = new();
    static readonly HttpClient client = new(clientHandler);

    public static async void SetSessionEnglish(string schoolUrl, string? sessionToken = null)
    {
        Uri langUri = new($"https://{schoolUrl}/ajax/ajax_lang.jsp?lang=EN");

        // Perform web request for language change
        using var langRequest = new HttpRequestMessage(new HttpMethod("GET"), langUri);
        if (sessionToken != null) langRequest.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        await client.SendAsync(langRequest);
    }

}
