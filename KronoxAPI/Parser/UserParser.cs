using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using KronoxAPI.Exceptions;
using HtmlAgilityPack;

namespace KronoxAPI.Parser;

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
    public static Dictionary<string, string> ParseToNames(HtmlDocument loginLandingPageHtml)
    {
        // Check if login was successful by trying to parse the website
        var nameAndUsername = loginLandingPageHtml.DocumentNode.SelectSingleNode("//*[@id='topnav']/span");

        if (nameAndUsername == null)
        {
            if (loginLandingPageHtml.DocumentNode.SelectSingleNode("/html/body/div[2]/div[4]/div/span") != null)
            {
                throw new LoginException("Kronox rejected the login attempt due to bad credentials or something else on their end.");
            }

            throw new ParseException("An error occurred while parsing the login page information. Please ensure the format of the given HTML conforms with Kronox's default structure.");
        }

        var nameAndUsernameMatch = Regex.Match(nameAndUsername.InnerHtml, @"Inloggad som: (?<name>\D*)\d* \[(?<username>.*)\]");
        var name = "N/A";
        var username = "N/A";

        if (nameAndUsernameMatch.Groups.ContainsKey("name"))
            name = nameAndUsernameMatch.Groups["name"].Value;

        if (nameAndUsernameMatch.Groups.ContainsKey("username"))
            username = nameAndUsernameMatch.Groups["username"].Value;

        return new Dictionary<string, string> { { "name", name }, { "username", username } };
    }
}
