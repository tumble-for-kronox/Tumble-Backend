using KronoxAPI.Utilities;
using KronoxAPI.Model.Users;
using System.Net.Http.Headers;
using KronoxAPI.Model.Scheduling;

namespace KronoxAPI.Controller;

/// <summary>
/// Controller for all communication with the Kronox website and its APIs.
/// </summary>
public static class KronoxFetchController
{
    static readonly MultiRequest client;

    static KronoxFetchController()
    {
        client = new MultiRequest();
    }

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
    public static async Task<string> GetSchedule(string[] scheduleId, string[] schoolUrls, LangEnum? language, string? sessionToken, DateTime? startDate)
    {
        string parsedDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "idag";
        LangEnum parsedLang = language == null ? LangEnum.Sv : language;

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/setup/jsp/SchemaXML.jsp?startDatum={parsedDate}&intervallTyp=m&intervallAntal=6&sprak={parsedLang}&sokMedAND=true&forklaringar=true&resurser={string.Join(',', scheduleId)}").ToArray();

        Dictionary<string, string>? requestHeaders = sessionToken == null ? null : new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new HttpMethod("POST"), requestHeaders: requestHeaders);
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
    public static async Task<string> GetProgrammes(string searchQuery, string[] schoolUrls, string? sessionToken)
    {
        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_sokResurser.jsp?sokord={searchQuery}&startDatum=idag&slutDatum=&intervallTyp=m&intervallAntal=6").ToArray();

        Dictionary<string, string>? requestHeaders = sessionToken == null ? null : new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new HttpMethod("GET"), requestHeaders: requestHeaders);
        string content = await response.Content.ReadAsStringAsync();
        return content;
    }

    /// <summary>
    /// Https fetch to Kronox, getting the HTML page with <see cref="UserEvent"/> information.
    /// </summary>
    /// <param name="schoolUrl"></param>
    /// <param name="sessionToken"></param>
    /// <returns><see cref="string"/> HTML page, which carries <see cref="UserEvent"/> data.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<string> GetUserEvents(string[] schoolUrls, string sessionToken)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        }

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/aktivitetsanmalan.jsp").ToArray();
        // Perform web request
        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
