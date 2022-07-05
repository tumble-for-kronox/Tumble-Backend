using KronoxAPI.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using KronoxAPI.Model.Schools;

namespace KronoxAPI.Controller
{
    public static class KronoxPushController
    {
        static readonly HttpClientHandler clientHandler = new();
        static readonly HttpClient client = new(clientHandler);

        /// <summary>
        /// Login a user, using the <paramref name="username"/> and <paramref name="password"/> supplied.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="schoolUrl"></param>
        /// <returns><see cref="string"/> containing the login session token.</returns>
        public static async Task<LoginResponse> Login(string username, string password, string schoolUrl)
        {
            // Create CookieContainer to store eventual response cookies in
            CookieContainer cookies = new();
            // Make sure the client has a new, empty CookieContainer
            clientHandler.CookieContainer = cookies;

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
            IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
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
            string stringContent = $"op=anmal&aktivitetsTillfallesId={WebUtility.UrlEncode(userEventId)}";

            // Perform web request
            HttpRequestMessage request = BuildGetRequestWithContent(uri, stringContent, sessionToken);
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
            string stringContent = $"op=avanmal&deltagarMojlighetsId={WebUtility.UrlEncode(userEventId)}";

            // Perform web request
            HttpRequestMessage request = BuildGetRequestWithContent(uri, stringContent, sessionToken);
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
            string stringContent = $"op=laggTillStod&stodId={WebUtility.UrlEncode(supportId)}&deltagarId={WebUtility.UrlEncode(participatorId)}";

            // Perform web request
            HttpRequestMessage request = BuildGetRequestWithContent(uri, stringContent, sessionToken);
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
            string stringContent = $"op=tabortStod&aktivitetsTillfallesId={WebUtility.UrlEncode(userEventId)}stodId={WebUtility.UrlEncode(supportId)}&deltagarId={WebUtility.UrlEncode(participatorId)}";
            
            // Perform web request
            HttpRequestMessage request = BuildGetRequestWithContent(uri, stringContent, sessionToken);
            HttpResponseMessage response = await client.SendAsync(request);
            
            if (response.StatusCode != HttpStatusCode.OK) return false;

            return true;
        }

        private static HttpRequestMessage BuildGetRequestWithContent(Uri uri, string stringContent, string sessionToken)
        {
            using HttpRequestMessage request = new(new HttpMethod("GET"), uri);
            request.Content = new StringContent(stringContent);
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
            request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");

            return request;
        }
    }
}
