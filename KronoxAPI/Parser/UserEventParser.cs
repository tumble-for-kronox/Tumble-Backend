﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using KronoxAPI.Model.Users;
using KronoxAPI.Exceptions;
using KronoxAPI.Extensions;
using HtmlAgilityPack;

namespace KronoxAPI.Parser;

public class UserEventParser
{
    /// <summary>
    /// Parse a standard Kronox HTML response of *all* user events into a <see cref="Dictionary{String, List{UserEvent}}"/>.
    /// <para>
    /// This function assumes the HTML is a direct return from Kronox's website.
    /// </para>
    /// </summary>
    /// <returns>
    /// The dict of lists <see cref="UserEvent"/> objects that correspond to the user events found in <paramref name="userEventsHtml"/>.
    /// <para>
    /// The dict keys are as follows:
    /// <list type="bullet">
    /// <item>registered</item>
    /// <item>unregistered</item>
    /// <item>upcoming</item>
    /// </list>
    /// </para>
    /// </returns>
    /// <exception cref="ParseException"></exception>
    /// <exception cref="LoginException"></exception>
    public static Dictionary<string, List<UserEvent>> ParseToDict(HtmlDocument userEventsHtml)
    {
        Dictionary<string, List<UserEvent>> userEvents = new() { { "registered", new List<UserEvent>() }, { "unregistered", new List<UserEvent>() }, { "upcoming", new List<UserEvent>() } };

        // Fetch all individual user event div nodes for scraping.
        try
        {
            IEnumerable<HtmlNode> registeredEvents = userEventsHtml.DocumentNode.SelectSingleNode("/html/body/div[2]/div[4]/div/div[1]/div").SelectNodes("*");
            IEnumerable<HtmlNode> unregisteredEvents = userEventsHtml.DocumentNode.SelectSingleNode("/html/body/div[2]/div[4]/div/div[2]/div").SelectNodes("*");
            IEnumerable<HtmlNode> upcomingEvents = userEventsHtml.DocumentNode.SelectSingleNode("/html/body/div[2]/div[4]/div/div[3]/div").SelectNodes("*");

            if (registeredEvents != null)
            {
                foreach (var registeredEvent in registeredEvents)
                {
                    userEvents["registered"].Add(ParseAvailableEvent(registeredEvent, true));
                }
            }

            if (unregisteredEvents != null)
            {
                foreach (var unregisteredEvent in unregisteredEvents)
                {
                    userEvents["unregistered"].Add(ParseAvailableEvent(unregisteredEvent, false));
                }
            }

            if (upcomingEvents == null) return userEvents;
            foreach (var upcomingEvent in upcomingEvents)
            {
                userEvents["upcoming"].Add(ParseUpcomingEvent(upcomingEvent));
            }

            return userEvents;
        }
        catch (NullReferenceException e)
        {
            if (userEventsHtml.SessionExpired())
                throw new LoginException($"Kronox rejected the login attempt due to bad credentials or something else on their end. Below is the HTML that caused the issue:\n\n{userEventsHtml.Text}", e);

            throw new ParseException("An error occurred while attempting to parse registered, unregistered, and upcoming events.", e);
        }

    }

    /// <summary>
    /// Scrape a single Kronox event HTML div element (has the class "tentamen-post") that represents an available user event into an <see cref="AvailableUserEvent"/> object.
    /// </summary>
    /// <param name="userEventHtmlDiv"></param>
    /// <param name="isRegistered"></param>
    /// <returns>The <see cref="AvailableUserEvent"/> object with the same data as the user event div from <paramref name="userEventHtmlDiv"/>.</returns>
    /// <exception cref="ParseException"></exception>
    private static AvailableUserEvent ParseAvailableEvent(HtmlNode userEventHtmlDiv, bool isRegistered)
    {
        // Variables needed to construct the AvailableUserEvent in the end.
        var supportAvailable = false;
        var mustChooseLocation = false;
        string? id = null;
        string? participatorId = null;
        string? supportId = null;
        string title;
        string type;
        var anonymousCode = string.Empty;
        DateTime lastSignupDate;
        DateTime startTime;
        DateTime endTime;

        // Raw data scraped directly from the response. Will be used to construct some variables needed.
        string rawDate;
        string rawStartTime;
        string rawEndTime;
        string rawLastSignupDate;

        // Scrape title directly.
        try
        {
            title = userEventHtmlDiv.Descendants("b").First().InnerText.Trim();
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex);
            throw new ParseException("An error occured while parsing the title of a user event.", ex);
        }

        // Get the button elements. This will fetch things like the "signup" and "support" buttons.
        IEnumerable<HtmlNode> buttonElements = userEventHtmlDiv.Descendants("a");

        // Scrape necessary ids from function calls and determine if support is available.
        foreach (var button in buttonElements)
        {
            // The button is a support button.
            if (button.GetAttributeValue("onclick", "").ToLowerInvariant().Contains("stod"))
            {
                supportAvailable = true;
                // Fetch the 2 ids needed for support adding from the button using regex.
                var supportAndParticipatorIds = Regex.Match(HttpUtility.HtmlDecode(button.GetAttributeValue("onclick", "").ToLowerInvariant()), @"visastod\('(.*?)','(.*?)'\)");

                participatorId = supportAndParticipatorIds.Groups[1].Value;
                supportId = supportAndParticipatorIds.Groups[2].Value;
                continue;
            }

            // The button is a "sign off" button.
            if (button.GetAttributeValue("onclick", "").ToLowerInvariant().Contains("avanmal"))
            {
                id = Regex.Match(HttpUtility.HtmlDecode(button.GetAttributeValue("onclick", "").ToLowerInvariant()), @"avanmal\(.*?, '(.*?)'\)").Groups[1].Value;
                continue;
            }

            // The button is a "sign up" button.
            if (!button.GetAttributeValue("onclick", "").ToLowerInvariant().Contains("anmal")) continue;
            var eventIdAndLocationChoice = Regex.Match(HttpUtility.HtmlDecode(button.GetAttributeValue("onclick", "").ToLowerInvariant()), @"anmal\('(.*?)',\s*(.*?)\)");
            id = eventIdAndLocationChoice.Groups[1].Value;
            if (eventIdAndLocationChoice.Groups[2].Value == "true") mustChooseLocation = true;
        }

        List<HtmlNode> dataNodes = userEventHtmlDiv.Descendants("div").SkipWhile(el => !el.InnerText.ToLowerInvariant().Contains("test date")).ToList();

        // Scrape all the raw data directly from the HTML nodes.
        try
        {
            rawDate = Regex.Match(dataNodes[0].InnerText, @"Test Date\s*:\s*(.*)").Groups[1].Value;
            rawStartTime = Regex.Match(dataNodes[1].InnerText, @"Start\s*:\s*(.*)").Groups[1].Value;
            rawEndTime = Regex.Match(dataNodes[2].InnerText, @"End\s*:\s*(.*)").Groups[1].Value;
            rawLastSignupDate = Regex.Match(dataNodes[3].InnerText, @"Registration closes\s*:\s*(.*)").Groups[1].Value;
            type = Regex.Match(dataNodes[4].InnerText, @"Test Type\s*:\s*(.*)").Groups[1].Value;

            if (isRegistered)
                anonymousCode = Regex.Match(dataNodes[5].InnerText, @"Anonymous Id\s*:\s*(.*)").Groups[1].Value;

        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine(ex);
            throw new ParseException("An error occured while scraping the date, start/end times, last signup date, type of event, or anonymous user code.", ex);
        }

        // Start turning raw data into usable types.
        try
        {
            lastSignupDate = DateTime.Parse(rawLastSignupDate);
            startTime = DateTime.Parse(rawDate + " " + rawStartTime);
            endTime = DateTime.Parse(rawDate + " " + rawEndTime);
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex);
            throw new ParseException("An error occured while parsing the format of the last signup date or the start/end times and dates.");
        }

        return new AvailableUserEvent(title, type, lastSignupDate, startTime, endTime, id, participatorId, supportId, anonymousCode, isRegistered, supportAvailable, mustChooseLocation);
    }

    /// <summary>
    /// Parse a single Kronox event HTML div element (has the class "tentamen-post") that represents an upcoming event into an <see cref="UpcomingUserEvent"/> object.
    /// </summary>
    /// <param name="userEventHtmlDiv"></param>
    /// <returns>The <see cref="UpcomingUserEvent"/> corresponding to the input <paramref name="userEventHtmlDiv"/>.</returns>
    /// <exception cref="ParseException"></exception>
    private static UpcomingUserEvent ParseUpcomingEvent(HtmlNode userEventHtmlDiv)
    {
        // Variables needed to construct the UpcomingUserEvent in the end.
        string title;
        string type;
        DateTime firstSignupDate;
        DateTime startTime;
        DateTime endTime;

        // Raw data scraped directly from the response. Will be used to construct some variables needed.
        string rawDate;
        string rawStartTime;
        string rawEndTime;
        string rawFirstSignupDate;

        // Scrape title directly.
        try
        {
            title = userEventHtmlDiv.Descendants("b").First().InnerText.Trim();
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex);
            throw new ParseException("An error occured while scraping the title of a user event.", ex);
        }

        List<HtmlNode> dataNodes = userEventHtmlDiv.Descendants("div").Skip(1).ToList();

        // Scrape all the raw data directly from the HTML nodes.
        try
        {
            rawDate = Regex.Match(dataNodes[0].InnerText, @"Test Date\s*:\s*(.*)").Groups[1].Value;
            rawStartTime = Regex.Match(dataNodes[1].InnerText, @"Start\s*:\s*(.*)").Groups[1].Value;
            rawEndTime = Regex.Match(dataNodes[2].InnerText, @"End\s*:\s*(.*)").Groups[1].Value;
            rawFirstSignupDate = Regex.Match(dataNodes[3].InnerText, @"Registration opens\s*:\s*(.*)").Groups[1].Value;
            type = Regex.Match(dataNodes[4].InnerText, @"Test Type\s*:\s*(.*)").Groups[1].Value;
        }
        catch (IndexOutOfRangeException ex)
        {
            Console.WriteLine(ex);
            throw new ParseException("An error occured while scraping the date, start/end times, last signup date, or type of event.", ex);
        }

        // Start turning raw data into usable types.
        try
        {
            firstSignupDate = DateTime.Parse(rawFirstSignupDate);
            startTime = DateTime.Parse(rawDate + " " + rawStartTime);
            endTime = DateTime.Parse(rawDate + " " + rawEndTime);
        }
        catch (FormatException ex)
        {
            Console.WriteLine(ex);
            throw new ParseException("An error occured while parsing the last signup date or start/end times and dates.", ex);
        }

        return new UpcomingUserEvent(title, type, firstSignupDate, startTime, endTime);
    }
}