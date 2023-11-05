using KronoxAPI.Model.Response;
using System.Net;
using System.Web;
using TumbleHttpClient;
using KronoxAPI.Extensions;
using KronoxAPI.Exceptions;

namespace KronoxAPI.Controller;

public static class KronoxPushController
{
    /// <summary>
    /// Login a user, using the <paramref name="username"/> and <paramref name="password"/> supplied.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="schoolUrl"></param>
    /// <returns><see cref="string"/> containing the login session token.</returns>
    public static async Task<LoginResponse?> Login(IKronoxRequestClient client, string username, string password)
    {
        string endpoint = "login_do.jsp";
        HttpRequestMessage req = new(new("POST"), endpoint)
        {
            Content = new StringContent($"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}")
        };
        req.Content.Headers.ContentType = new("application/x-www-form-urlencoded");

        HttpResponseMessage response = await client.SendAsync(req);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        string? sessionToken = (client.CookieContainer.GetCookie(client.BaseUrl!, "JSESSIONID")?.Value) ?? throw new LoginException($"No session token was found after logging in, failed to log in {username}.");

        // Return the relevant cookie value (login session token)
        return new LoginResponse(sessionToken, await response.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Push information regarding registering a user (based on <paramref name="sessionToken"/>) for a specific event to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="userEventId"></param>

    public static async Task<bool> UserEventRegister(IKronoxRequestClient client, string userEventId)
    {
        string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "anmal";
        query["aktivitetsTillfallesId"] = userEventId;
        query["ort"] = "";

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK) return false;

        return true;
    }

    /// <summary>
    /// Push information regarding unregistering a user (based on <paramref name="sessionToken"/>) for a specific event to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="sessionToken"></param>
    /// <param name="userEventId"></param>
    /// <returns>A bool showing whether the information was correctly pushed to the backend.</returns>
    public static async Task<bool> UserEventUnregister(IKronoxRequestClient client, string userEventId)
    {
        string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "avanmal";
        query["deltagarMojlighetsId"] = userEventId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK) return false;

        return true;
    }

    /// <summary>
    /// Add support for a user (based on <paramref name="sessionToken"/>) to a given event and push the data to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="sessionToken"></param>
    /// <param name="participatorId"></param>
    /// <param name="supportId"></param>
    /// <returns>A bool showing whether the information was correctly pushed to the backend.</returns>
    public static async Task<bool> UserEventAddSupport(KronoxRequestClient client, string participatorId, string supportId)
    {
        string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "laggTillStod";
        query["stodId"] = supportId;
        query["deltagarId"] = participatorId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK) return false;

        return true;
    }

    /// <summary>
    /// Remove support for a user (based on <paramref name="sessionToken"/>) to a given event and push the data to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="sessionToken"></param>
    /// <param name="userEventId"></param>
    /// <param name="participatorId"></param>
    /// <param name="supportId"></param>
    /// <returns>A bool showing whether the information was correctly pushed to the backend.</returns>
    public static async Task<bool> UserEventRemoveSupport(KronoxRequestClient client, string userEventId, string participatorId, string supportId)
    {
        string endpoint = "ajax/ajax_aktivitetsanmalan.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "tabortStod";
        query["aktivitetsTillfallesId"] = userEventId;
        query["stodId"] = supportId;
        query["deltagarId"] = participatorId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK) return false;

        return true;
    }
}
