using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using KronoxAPI.Exceptions;
using HtmlAgilityPack;

namespace KronoxAPI.Parser
{
    /// <summary>
    /// Methods for managing the parsing of Kronox user data.
    /// </summary>
    public static class UserParser
    {
        /// <summary>
        /// Parses the HTML return of a Kronox login to determine if the login succeeded and fetching the user related data if login succeeded.
        /// </summary>
        /// <param name="loginLandingPageHtml"></param>
        /// <returns>Dictionary with two keys: <c>name</c> and <c>username</c> each containing the name and username of the user logged in.</returns>
        /// <exception cref="LoginException"></exception>
        public static Dictionary<string, string> ParseToNames(string loginLandingPageHtml)
        {
            HtmlDocument document = new();
            document.LoadHtml(loginLandingPageHtml);

            // Check if login was successful by trying to parse the website
            string nameAndUsername;

            try
            {
                nameAndUsername = document.DocumentNode
                    .SelectNodes("//*[@id='topnav']/span")
                    .First()
                    .InnerHtml;
            }
            catch (ArgumentNullException)
            {
                throw new LoginException("Login failed.");
            }

            Match nameAndUsernameMatch = Regex.Match(nameAndUsername, @"Inloggad som: (?<name>\D*)\d* \[(?<username>.*)\]");
            string name = "N/A";
            string username = "N/A";

            if (nameAndUsernameMatch.Groups.ContainsKey("name"))
                name = nameAndUsernameMatch.Groups["name"].Value;

            if (nameAndUsernameMatch.Groups.ContainsKey("username"))
                username = nameAndUsernameMatch.Groups["username"].Value;

            return new() { { "name", name }, { "username", username } };
        }
    }
}
