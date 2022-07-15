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
using HtmlAgilityPack;


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
    private readonly string _id;
    private readonly string _name;
    private readonly string _url;
    private readonly bool _loginRequired;

    public string Id => _id;

    public string Name => _name;

    public string Url => _url;

    public bool LoginRequired => _loginRequired;

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
    /// <param name="url"></param>
    /// <param name="loginRequired"></param>
    public School(string id, string name, string url, bool loginRequired)
    {
        this._id = id;
        this._url = url;
        this._loginRequired = loginRequired;
        this._name = name;
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
    /// <para>
    /// If no value is passed for <paramref name="sessionToken"/> no session token will be used when fetching the schedule. This will only function if the schedule is publicly available.
    /// </para>
    /// </summary>
    /// <param name="id"></param>
    /// <param name="language"></param>
    /// <param name="sessionToken"></param>
    /// <param name="startDate"></param>
    /// <returns></returns>
    public Schedule FetchSchedule(string id, LangEnum? language, string? sessionToken, DateTime? startDate)
    {
        string scheduleXmlString = KronoxFetchController.GetSchedule(id, Url, language, sessionToken, startDate).Result;
        XDocument scheduleXml = XDocument.Parse(scheduleXmlString);
        List<Day> scheduleDaysOfEvents = ScheduleParser.ParseToDays(scheduleXml);

        return new Schedule(id, scheduleDaysOfEvents);
    }

    /// <summary>
    /// Searches between all schedules available on the relative school, using <paramref name="searchQuery"/> to search directly in Kronox's data.
    /// </summary>
    /// <param name="searchQuery"></param>
    /// <param name="sessionToken"></param>
    /// <returns>The <see cref="List{Programme}"/> objects equivalent to the search results found in Kronox's database.</returns>
    public List<Programme> SearchProgrammes(string searchQuery, string? sessionToken)
    {
        string searchResultsHtml = KronoxFetchController.GetProgrammes(searchQuery, Url, sessionToken).Result;
        return SearchParser.ParseToProgrammes(searchResultsHtml);
    }

    /// <summary>
    /// Uses the provided <paramref name="username"/> and <paramref name="password"/> to log the user into Kronox.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns>
    /// The <see cref="User"/> object with the found name, username (shortened username, that Kronox uses) and the active session token.
    /// Be aware that Kronox's session tokens expire after 15 minutes.
    /// </returns>
    public User Login(string username, string password)
    {
        Response.LoginResponse loginResponse = KronoxPushController.Login(username, password, Url).Result;
        HtmlDocument loginResponseHtmlDocument = new();
        loginResponseHtmlDocument.LoadHtml(loginResponse.htmlResult);

        Dictionary<string, string> result = UserParser.ParseToNames(loginResponseHtmlDocument);

        return new User(result["name"], result["username"], loginResponse.sessionToken);
    }
}
