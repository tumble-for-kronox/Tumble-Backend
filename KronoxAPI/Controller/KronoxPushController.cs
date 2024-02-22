using KronoxAPI.Model.Response;
using System.Net;
using System.Web;
using TumbleHttpClient;
using KronoxAPI.Extensions;
using KronoxAPI.Exceptions;
using KronoxAPI.Utilities;

namespace KronoxAPI.Controller;

public static class KronoxPushController
{
    /// <summary>
    /// Login a user, using the <paramref name="username"/> and <paramref name="password"/> supplied.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns><see cref="string"/> containing the login session token.</returns>
    public static async Task<LoginResponse?> LoginAsync(IKronoxRequestClient client, string username, string password)
    {
        const string endpoint = "login_do.jsp";
        var postData = $"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}";

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Post, endpoint, new Dictionary<string, string>(), postData);

        var sessionToken = (client.CookieContainer.GetCookie(client.BaseUrl!, "JSESSIONID")?.Value) ?? throw new LoginException($"No session token was found after logging in, failed to log in {username}.");

        return new LoginResponse(sessionToken, await response.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Push information regarding registering a user (based on <paramref name="sessionToken"/>) for a specific event to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userEventId"></param>
    public static async Task<bool> UserEventRegisterAsync(IKronoxRequestClient client, string userEventId)
    {
        const string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";
        var parameters = new Dictionary<string, string>
        {
            ["op"] = "anmal",
            ["aktivitetsTillfallesId"] = userEventId,
            ["ort"] = ""
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return response.StatusCode.Equals(HttpStatusCode.OK);
    }

    /// <summary>
    /// Push information regarding unregistering a user (based on <paramref name="sessionToken"/>) for a specific event to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userEventId"></param>
    /// <returns>A bool showing whether the information was correctly pushed to the backend.</returns>
    public static async Task<bool> UserEventUnregisterAsync(IKronoxRequestClient client, string userEventId)
    {
        const string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";
        var parameters = new Dictionary<string, string>
        {
            ["op"] = "avanmal",
            ["deltagarMojlighetsId"] = userEventId
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return response.StatusCode.Equals(HttpStatusCode.OK);
    }

    /// <summary>
    /// Add support for a user (based on <paramref name="sessionToken"/>) to a given event and push the data to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="participatorId"></param>
    /// <param name="supportId"></param>
    /// <returns>A bool showing whether the information was correctly pushed to the backend.</returns>
    public static async Task<bool> UserEventAddSupportAsync(KronoxRequestClient client, string participatorId, string supportId)
    {
        const string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";
        var parameters = new Dictionary<string, string>
        {
            ["op"] = "laggTillStod",
            ["stodId"] = supportId,
            ["deltagarId"] = participatorId
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return response.StatusCode.Equals(HttpStatusCode.OK);
    }

    /// <summary>
    /// Remove support for a user (based on <paramref name="sessionToken"/>) to a given event and push the data to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="userEventId"></param>
    /// <param name="participatorId"></param>
    /// <param name="supportId"></param>
    /// <returns>A bool showing whether the information was correctly pushed to the backend.</returns>
    public static async Task<bool> UserEventRemoveSupportAsync(KronoxRequestClient client, string userEventId, string participatorId, string supportId)
    {
        string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";
        var parameters = new Dictionary<string, string>
        {
            ["op"] = "tabortStod",
            ["aktivitetsTillfallesId"] = userEventId,
            ["stodId"] = supportId,
            ["deltagarId"] = participatorId
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return response.StatusCode.Equals(HttpStatusCode.OK);
    }
}
