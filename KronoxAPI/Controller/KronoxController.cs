using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Controller
{
    // MAKE THIS SUPPORT SESSION TOKEN HEADERS!!
    
    /// <summary>
    /// Controller for all communication with the Kronox website and its APIs.
    /// </summary>
    public static class KronoxController
    {
        static HttpClientHandler clientHandler = new HttpClientHandler();
        static readonly HttpClient client = new HttpClient(clientHandler);

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
        public static async Task<string> GetSchedule(string scheduleId, string schoolUrl, string? sessionToken, DateTime? startDate)
        {
            string parsedDate = startDate.HasValue ? startDate.Value.ToString("yyyy-MM-dd") : "idag";
            string uri = $"https://{schoolUrl}/setup/jsp/SchemaICAL.ics?startDatum={parsedDate}&intervallTyp=m&intervallAntal=6&sprak=SV&sokMedAND=true&forklaringar=true&resurser={scheduleId}";

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
        /// Https fetch to Kronox, getting the HTML page with <see cref="Model.User.UserEvent"/> information.
        /// </summary>
        /// <param name="schoolUrl"></param>
        /// <param name="sessionToken"></param>
        /// <returns><see cref="string"/> HTML page, which carries <see cref="Model.User.UserEvent"/> data.</returns>
        public static async Task<string> GetUserEvents(string schoolUrl, string sessionToken)
        {
            Uri uri = new($"https://{schoolUrl}/aktivitetsanmalan.jsp");

            // Perform web request
            using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
            request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Login a user, using the <paramref name="username"/> and <paramref name="password"/> supplied.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="schoolUrl"></param>
        /// <returns><see cref="string"/> containing the login session token.</returns>
        public static async Task<string> Login(string username, string password, string schoolUrl)
        {
            // Create CookieContainer to store eventual response cookies in
            CookieContainer cookies = new();
            // Make sure the client has a new, empty CookieContainer
            clientHandler.CookieContainer = cookies;

            Uri uri = new($"https://{schoolUrl}/login_do.jsp");

            // Perform web request
            using var request = new HttpRequestMessage(new HttpMethod("POST"), uri);
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
            return sessionToken;
        }
    }
}
