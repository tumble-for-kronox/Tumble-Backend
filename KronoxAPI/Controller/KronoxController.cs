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
        static readonly HttpClient client = new HttpClient();

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

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            if (sessionToken != null) request.Headers.Add("cookies", $"JSESSIONID={sessionToken};");

            var response = await client.SendAsync(request);
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
            string uri = $"https://kronox.hkr.se/ajax/ajax_sokResurser.jsp?sokord={searchQuery}&startDatum=idag&slutDatum=&intervallTyp=m&intervallAntal=6";

            string responseBody = await client.GetStringAsync(uri);
            return responseBody;
        }

        public static async Task<string> GetUserEvents(string sessionToken)
        {
            return "";
        }

        public static async Task<string> Login(string username, string password, string schoolUrl)
        {
            using var request = new HttpRequestMessage(new HttpMethod("POST"), $"https://{schoolUrl}/login_do.jsp");
            request.Content = new StringContent($"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}");
            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
