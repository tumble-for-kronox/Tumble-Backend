using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KronoxAPI.Utilities;
using KronoxAPI.Model.Users;

namespace KronoxAPI.Controller;

/// <summary>
/// Controller for all communication with the Kronox website and its APIs.
/// </summary>
public static class KronoxFetchController
{
    static readonly HttpClientHandler clientHandler = new();
    static readonly HttpClient client = new(clientHandler);

    /// <summary>
    /// Fetch a schedule from Kronox's database, that starts at <paramref name="startDate"/> and goes 6 months forward.
    /// <para>
    /// <paramref name="schoolUrl"/> is the domain name for the given school.
    /// <paramref name="sessionToken"/> is the session token for logged in users.
    /// </para>
    /// </summary>
    /// <param name="scheduleId"></param>
    /// <param name="schoolUrl"></param>
    /// <param name="sessionToken"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public static async Task<string> GetSchedule(string[] scheduleId, string schoolUrl, LangEnum? language, string? sessionToken, DateTime? startDate)
    {
        string parsedDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "idag";
        LangEnum parsedLang = language == null ? LangEnum.Sv : language;

        string uri = $"https://{schoolUrl}/setup/jsp/SchemaXML.jsp?startDatum={parsedDate}&intervallTyp=m&intervallAntal=6&sprak={parsedLang}&sokMedAND=true&forklaringar=true&resurser={string.Join(',', scheduleId)}";

        using var request = new HttpRequestMessage(HttpMethod.Get, uri);
        if (sessionToken != null) request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");

        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Fetch all search results from Kronox's database that come from the given <paramref name="searchQuery"/>.
    /// <para>
    /// <paramref name="schoolUrl"/> is the domain name for the given school.
    /// <paramref name="sessionToken"/> is the session token for logged in users.
    /// </para>
    /// </summary>
    /// <param name="searchQuery"></param>
    /// <param name="schoolUrl"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    public static async Task<string> GetProgrammes(string searchQuery, string schoolUrl, string? sessionToken)
    {
        Uri uri = new($"https://{schoolUrl}/ajax/ajax_sokResurser.jsp?sokord={searchQuery}&startDatum=idag&slutDatum=&intervallTyp=m&intervallAntal=6");

        // Perform web request
        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        if (sessionToken != null) request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Https fetch to Kronox, getting the HTML page with <see cref="UserEvent"/> information.
    /// </summary>
    /// <param name="schoolUrl"></param>
    /// <param name="sessionToken"></param>
    /// <returns><see cref="string"/> HTML page, which carries <see cref="UserEvent"/> data.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<string> GetUserEvents(string schoolUrl, string sessionToken)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        Uri uri = new($"https://{schoolUrl}/aktivitetsanmalan.jsp");

        // Perform web request
        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
