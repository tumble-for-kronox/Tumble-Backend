using KronoxAPI.Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
    }
}
