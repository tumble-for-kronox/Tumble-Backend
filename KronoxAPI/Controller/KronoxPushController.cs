using KronoxAPI.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http.Headers;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using KronoxAPI.Model.Schools;

namespace KronoxAPI.Controller;

public static class KronoxPushController
{
    static readonly HttpClient client;
    static readonly HttpClientHandler clientHandler;

    static KronoxPushController()
    {
        clientHandler = new HttpClientHandler();
        client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
    }

    /// <summary>
    /// Login a user, using the <paramref name="username"/> and <paramref name="password"/> supplied.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <param name="schoolUrl"></param>
    /// <returns><see cref="string"/> containing the login session token.</returns>
    public static async Task<LoginResponse> Login(string username, string password, string schoolUrl)
    {
        // MDH currently has a sporadic domain name, causing us to resort
        // to dynamic handling of the string in login requests
        Uri uri = new($"https://{schoolUrl}/login_do.jsp");

        // Perform web request
        using HttpRequestMessage request = new(new HttpMethod("POST"), uri);
        request.Content = new StringContent($"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}");
        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
        HttpResponseMessage response = await client.SendAsync(request);

        // Handle response, making sure it's good and preparing to strip session cookie
        response.EnsureSuccessStatusCode();
        string sessionToken = string.Empty;

        // Fetch cookies as iterable from CookieContainer
        IEnumerable<Cookie> responseCookies = clientHandler.CookieContainer.GetCookies(uri).Cast<Cookie>();
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

    public static async Task<bool> UserEventRegister(School school, string sessionToken, string userEventId)
    {
        Uri uri = new($"https://{school.Url}/ajax/ajax_aktivitetsanmalan.jsp");

        // Add query parameters needed for the request.
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        query["op"] = "anmal";
        query["aktivitetsTillfallesId"] = userEventId;
        query["ort"] = "";

        // Perform web request
        HttpRequestMessage request = BuildGetRequestWithQueryParams(uri, query.ToString(), sessionToken);
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
    public static async Task<bool> UserEventUnregister(School school, string sessionToken, string userEventId)
    {
        Uri uri = new($"https://{school.Url}/ajax/ajax_aktivitetsanmalan.jsp");

        // Add query parameters needed for the request.
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        query["op"] = "avanmal";
        query["deltagarMojlighetsId"] = userEventId;

        // Perform web request
        HttpRequestMessage request = BuildGetRequestWithQueryParams(uri, query.ToString(), sessionToken);
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
    public static async Task<bool> UserEventAddSupport(School school, string sessionToken, string participatorId, string supportId)
    {
        Uri uri = new($"https://{school.Url}/ajax/ajax_aktivitetsanmalan.jsp");

        // Add query parameters needed for the request.
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        query["op"] = "laggTillStod";
        query["stodId"] = supportId;
        query["deltagarId"] = participatorId;

        // Perform web request
        HttpRequestMessage request = BuildGetRequestWithQueryParams(uri, query.ToString(), sessionToken);
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
    public static async Task<bool> UserEventRemoveSupport(School school, string sessionToken, string userEventId, string participatorId, string supportId)
    {
        Uri uri = new($"https://{school.Url}/ajax/ajax_aktivitetsanmalan.jsp");

        // Add query parameters needed for the request.
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);
        query["op"] = "tabortStod";
        query["aktivitetsTillfallesId"] = userEventId;
        query["stodId"] = supportId;
        query["deltagarId"] = participatorId;

        // Perform web request
        HttpRequestMessage request = BuildGetRequestWithQueryParams(uri, query.ToString(), sessionToken);
        HttpResponseMessage response = await client.SendAsync(request);

        if (response.StatusCode != HttpStatusCode.OK) return false;

        return true;
    }

    /// <summary>
    /// Helper method for combining URI with query params and building a HTTP GET request with them. Also adds the sessionToken cookie header.
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="queryParams"></param>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    private static HttpRequestMessage BuildGetRequestWithQueryParams(Uri uri, string queryParams, string sessionToken)
    {
        HttpRequestMessage request = new(new HttpMethod("GET"), uri + "?" + queryParams);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        return request;
    }
}
