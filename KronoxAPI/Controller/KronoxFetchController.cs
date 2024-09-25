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
    /// </para>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="scheduleId"></param>
    /// <param name="language"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public static async Task<string> GetScheduleAsync(IKronoxRequestClient client, string[] scheduleId, LangEnum? language, DateTime? startDate)
    {
        var parsedDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "idag";
        var parsedLang = language ?? LangEnum.Sv;
        const string endpoint = "setup/jsp/SchemaXML.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["startDatum"] = parsedDate,
            ["intervallTyp"] = "m",
            ["intervallAntal"] = "6",
            ["sprak"] = parsedLang.Value ?? "SV",
            ["sokMedAND"] = "false",
            ["forklaringar"] = "true",
            ["resurser"] = string.Join(',', scheduleId)
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Fetch all search results from Kronox's database that come from the given <paramref name="searchQuery"/>.
    /// <para>
    /// </para>
    /// </summary>
    /// <param name="client"></param>
    /// <param name="searchQuery"></param>
    /// <returns></returns>
    public static async Task<string> GetProgrammesAsync(IKronoxRequestClient client, string searchQuery)
    {
        const string endpoint = "ajax/ajax_sokResurser.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["sokord"] = searchQuery,
            ["startDatum"] = "idag",
            ["slutDatum"] = "",
            ["intervallTyp"] = "m",
            ["intervallAntal"] = "6"
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// Https fetch to Kronox, getting the HTML page with <see cref="UserEvent"/> information.
    /// </summary>
    /// <param name="client"></param>
    /// <returns><see cref="string"/> HTML page, which carries <see cref="UserEvent"/> data.</returns>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<string?> GetUserEventsAsync(IKronoxRequestClient client)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        const string endpoint = "aktivitetsanmalan.jsp";

        var parameters = new Dictionary<string, string>();

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return await response.Content.ReadAsStringAsync();
    }
}
