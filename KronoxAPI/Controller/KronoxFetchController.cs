using KronoxAPI.Utilities;
using KronoxAPI.Model.Users;
using TumbleHttpClient;
using System.Web;

namespace KronoxAPI.Controller;

/// <summary>
/// Controller for all communication with the Kronox website and its APIs.
/// </summary>
public static class KronoxFetchController
{
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
    public static async Task<string> GetSchedule(IKronoxRequestClient client, string[] scheduleId, LangEnum? language, DateTime? startDate)
    {
        string parsedDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "idag";
        LangEnum parsedLang = language ?? LangEnum.Sv;

        var query = HttpUtility.ParseQueryString("");
        query["startDatum"] = parsedDate;
        query["intervallTyp"] = "m";
        query["intervallAntal"] = "6";
        query["sprak"] = parsedLang.ToString();
        query["sokMedAND"] = "false";
        query["forklaringar"] = "true";
        query["resurser"] = string.Join(',', scheduleId);

        string fullPath = $"{endpoint}?{query}";
        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
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
    public static async Task<string> GetProgrammes(IKronoxRequestClient client, string searchQuery)
    {
        string endpoint = "ajax/ajax_sokResurser.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["sokord"] = searchQuery;
        query["startDatum"] = "idag";
        query["slutDatum"] = "";
        query["intervallTyp"] = "m";
        query["intervallAntal"] = "6";

        string fullPath = $"{endpoint}?{query}";
        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);
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
    public static async Task<string> GetUserEvents(IKronoxRequestClient client)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        string endpoint = "aktivitetsanmalan.jsp";

        HttpRequestMessage request = new(new HttpMethod("GET"), endpoint);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
