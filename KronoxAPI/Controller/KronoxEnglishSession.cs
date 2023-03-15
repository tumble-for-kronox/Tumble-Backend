using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Controller;

public static class KronoxEnglishSession
{
    static readonly HttpClientHandler clientHandler;
    static readonly HttpClient client;

    static KronoxEnglishSession()
    {
        clientHandler = new HttpClientHandler();
        client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
    }

    public static async void SetSessionEnglish(string schoolUrl, string? sessionToken = null)
    {
        Uri langUri = new($"https://{schoolUrl}/ajax/ajax_lang.jsp?lang=EN");

        // Perform web request for language change
        using var langRequest = new HttpRequestMessage(new HttpMethod("GET"), langUri);
        if (sessionToken != null) langRequest.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        await client.SendAsync(langRequest);
    }

}
