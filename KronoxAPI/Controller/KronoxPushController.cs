using KronoxAPI.Model.Response;
using System.Net;
using System.Web;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using KronoxAPI.Model.Schools;
using KronoxAPI.Utilities;
using System.Collections;
using KronoxAPI.Model.Users;

namespace KronoxAPI.Controller;

public static class KronoxPushController
{
    static readonly MultiRequest client;

    static KronoxPushController()
    {
        client = new MultiRequest();
    }

    /// <summary>
    /// Login a user, using the <paramref name="username"/> and <paramref name="password"/> supplied.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="schoolUrl"></param>
    /// <returns><see cref="string"/> containing the login session token.</returns>
    public static async Task<LoginResponse> Login(string username, string password, string[] schoolUrls)
    {
        string[] urls = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/login_do.jsp").ToArray();
        StringContent content = new($"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}");

        Dictionary<string, string> contentHeaders = new()
        {
            { "Content-Type", "application/x-www-form-urlencoded" }
        };

        HttpResponseMessage response = await client.SendAsync(urls, new HttpMethod("POST"), content: content, contentHeaders: contentHeaders);

        // Handle response, making sure it's good and preparing to strip session cookie
        response.EnsureSuccessStatusCode();
        string sessionToken = string.Empty;

        // Fetch cookies as iterable from CookieContainer
        IEnumerable<Cookie> responseCookies = client.clientHandler.CookieContainer.GetCookies(response.RequestMessage!.RequestUri!).Cast<Cookie>();
        foreach (Cookie cookie in responseCookies)
        {
            if (cookie.Name == "JSESSIONID") sessionToken = cookie.Value;
            // Make sure all the current cookies are expired, so there's no bleed between http requests on the same client
            cookie.Expired = true;
        }

        // Return the relevant cookie value (login session token)
        return new LoginResponse(sessionToken, await response.Content.ReadAsStringAsync());
    }

    /// <summary>
    /// Push information regarding registering a user (based on <paramref name="sessionToken"/>) for a specific event to Kronox's backend. If successful the user event data should be refetched to get the updated information.
    /// </summary>
    /// <param name="school"></param>
    /// <param name="userEventId"></param>

    public static async Task<bool> UserEventRegister(string[] schoolUrls, string sessionToken, string userEventId)
    {
        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_aktivitetsanmalan.jsp").ToArray();

        // Add query parameters needed for the request.
        Dictionary<string, string> queryParams = new()
        {
            { "op", "anmal" },
            { "aktivitetsTillfallesId", userEventId },
            { "ort", "" }
        };

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), queryParams: queryParams, requestHeaders: requestHeaders);

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
    public static async Task<bool> UserEventUnregister(string[] schoolUrls, string sessionToken, string userEventId)
    {
        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_aktivitetsanmalan.jsp").ToArray();

        // Add query parameters needed for the request.
        Dictionary<string, string> queryParams = new()
        {
            { "op", "avanmal" },
            { "deltagarMojlighetsId", userEventId }
        };

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), queryParams: queryParams, requestHeaders: requestHeaders);

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
    public static async Task<bool> UserEventAddSupport(string[] schoolUrls, string sessionToken, string participatorId, string supportId)
    {
        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_aktivitetsanmalan.jsp").ToArray();

        Dictionary<string, string> queryParams = new()
        {
            { "op", "laggTillStod" },
            { "stodId", supportId },
            { "deltagarId", participatorId }
        };

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), queryParams: queryParams, requestHeaders: requestHeaders);

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
    public static async Task<bool> UserEventRemoveSupport(string[] schoolUrls, string sessionToken, string userEventId, string participatorId, string supportId)
    {
        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_aktivitetsanmalan.jsp").ToArray();

        // Add query parameters needed for the request.
        Dictionary<string, string> queryParams = new()
        {
            { "op", "tabortStod" },
            { "aktivitetsTillfallesId", userEventId },
            { "stodId", supportId },
            { "deltagarId", participatorId }
        };

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), queryParams: queryParams, requestHeaders: requestHeaders);

        if (response.StatusCode != HttpStatusCode.OK) return false;

        return true;
    }
}
