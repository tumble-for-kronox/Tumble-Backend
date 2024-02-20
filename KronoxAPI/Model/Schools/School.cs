using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KronoxAPI.Controller;
using KronoxAPI.Model.Users;
using KronoxAPI.Model.Scheduling;
using KronoxAPI.Utilities;
using KronoxAPI.Parser;
using KronoxAPI.Exceptions;
using HtmlAgilityPack;
using System.Xml;
using KronoxAPI.Model.Booking;
using TumbleHttpClient;

namespace KronoxAPI.Model.Schools;

/// <summary>
/// <para>
/// Model for storing data regarding different Kronox schools.
/// </para>
/// <para>
/// Also contains a set of flow-based methods to ease interaction with Kronox. These methods are combinations of methods that are
/// available elsewhere in the library and combine their functionality. For custom flows see <see cref="Controller"/> and <see cref="Parser"/> modules.
/// </para>
/// <para>
/// The model also implements fetching and searching in Kronox's backend, to the relative school.
/// </para>
/// </summary>
public class School
{
    public SchoolEnum Id { get; }

    public string Name { get; }

    public string[] Urls { get; }

    public bool LoginRequired { get; }

    public SchoolResources Resources { get; }

    /// <summary>
    /// <para>
    /// For basic interaction with the schools in Kronox's database it is recommended
    /// to use <see cref="SchoolFactory"/> for constructing <see cref="School"/> instances.
    /// </para>
    /// <para>
    /// Using this constructor directly may create issues throughout the remainder of the
    /// implemented workflows, but is possible if needed.
    /// </para>
    /// <para>
    /// As an example using <c>SchoolFactory.Hkr()</c> will produce a school object setup to interact
    /// with HKR's Kronox services.
    /// </para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    /// <param name="urls"></param>
    /// <param name="loginRequired"></param>
    public School(SchoolEnum id, string name, string[] urls, bool loginRequired)
    {
        Id = id;
        Urls = urls;
        LoginRequired = loginRequired;
        Name = name;
        Resources = new SchoolResources();
    }

    /// <summary>
    /// <para>
    /// Fetch a specified schedule from the relative school, constructed through <see cref="SchoolFactory"/>.
    /// </para>
    /// 
    /// <para>
    /// If no value is passed for <paramref name="language"/> it will default to SWEDISH.
    /// </para>
    /// <para>
    /// If no value is passed for <paramref name="startDate"/> it will default to the current day.
    /// </para>
    /// </summary>
    /// <param name="scheduleIds"></param>
    /// <param name="language"></param>
    /// <param name="client"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    /// <exception cref="ParseException"></exception>
    public static async Task<Schedule> FetchScheduleAsync(IKronoxRequestClient client, string[] scheduleIds, LangEnum? language = null, DateTime? startDate = null)
    {
        try
        {
            var scheduleXmlString = await KronoxFetchController.GetScheduleAsync(client, scheduleIds, language, startDate);

            var scheduleXml = XDocument.Parse(scheduleXmlString);
            var scheduleDaysOfEvents = ScheduleParser.ParseToDays(scheduleXml);

            return new Schedule(scheduleIds, scheduleDaysOfEvents);

        }
        catch (Exception e) when (
            e is XmlException ||
            e is AggregateException
        )
        {
            Console.WriteLine(e.Message);
            throw new ParseException("The requested schedule could not be found or was corrupted.", e);
        }
    }

    /// <summary>
    /// Searches between all schedules available on the relative school, using <paramref name="searchQuery"/> to search directly in Kronox's data.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="searchQuery"></param>
    /// <returns>The <see cref="List{Programme}"/> objects equivalent to the search results found in Kronox's database.</returns>
    /// <exception cref="LoginException"></exception>
    public static async Task<List<Programme>> SearchProgrammesAsync(IKronoxRequestClient client, string searchQuery)
    {
        string searchResultsHtml = await KronoxFetchController.GetProgrammesAsync(client, searchQuery);
        return SearchParser.ParseToProgrammes(searchResultsHtml);
    }

    /// <summary>
    /// Uses the provided <paramref name="username"/> and <paramref name="password"/> to log the user into Kronox.
    /// </summary>
    /// <param name="client"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns>
    /// The <see cref="User"/> object with the found name, username (shortened username, that Kronox uses) and the active session token.
    /// Be aware that Kronox's session tokens expire after 15 minutes.
    /// </returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="ParseException"></exception>
    public static async Task<User?> LoginAsync(IKronoxRequestClient client, string username, string password)
    {
        var loginResponse = await KronoxPushController.LoginAsync(client, username, password);

        if (loginResponse == null)
        {
            return null;
        }

        HtmlDocument loginResponseHtmlDocument = new();
        loginResponseHtmlDocument.LoadHtml(loginResponse.HtmlResult);

        var result = UserParser.ParseToNames(loginResponseHtmlDocument);

        return new User(result["name"], result["username"], loginResponse.SessionToken);
    }

    /// <summary>
    /// Fetch a list of <see cref="UserEvent"/> from Kronox's database.
    /// </summary>
    /// <returns>List of events connected to the User.</returns>
    /// <exception cref="ParseException"></exception>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="HttpRequestException"></exception>
    public static async Task<Dictionary<string, List<UserEvent>>?> GetUserEventsAsync(IKronoxRequestClient client)
    {
        var userEventsHtmlResult = await KronoxFetchController.GetUserEventsAsync(client);

        if (userEventsHtmlResult == null)
        {
            return null;
        }

        HtmlDocument userEventHtmlDoc = new();
        userEventHtmlDoc.LoadHtml(userEventsHtmlResult);

        return UserEventParser.ParseToDict(userEventHtmlDoc);
    }

    public static async Task<bool> RefreshUserSessionAsync(IKronoxRequestClient client)
    {
        return await KronoxSessionController.RefreshSessionAsync(client);
    }
}
