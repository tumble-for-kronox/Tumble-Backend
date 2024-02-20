using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using KronoxAPI.Model.Scheduling;
using System.Web;

namespace KronoxAPI.Parser;

/// <summary>
/// All parsers built for parsing Schedule data and their related functions. Parses Kronox schedule XML standards into different combinations of objects.
/// </summary>
public static class ScheduleParser
{
    /// <summary>
    /// Parse entire scheduleXML (as per Kronox's standard XML layout) into <see cref="Event"/> Objects.
    /// </summary>
    /// <param name="scheduleXml"></param>
    /// <returns><see cref="List{Event}"/> where one <see cref="Event"/> object is one event on the schedule.</returns>
    public static List<Event> ParseToEvents(XDocument scheduleXml)
    {
        var teachersDict = GetScheduleTeachers(scheduleXml);
        var locationsDict = GetScheduleLocations(scheduleXml);
        var coursesDict = GetScheduleCourses(scheduleXml);

        return scheduleXml.Descendants("schemaPost").Select(e => XmlToEvent(e, teachersDict, locationsDict, coursesDict)).ToList();
    }

    /// <summary>
    /// Parse entire scheduleXML (as per Kronox's standard XML layout) into <see cref="Day"/> Objects.
    /// </summary>
    /// <param name="scheduleXml"></param>
    /// <returns>The <see cref="List{Day}"/> parsed from the XML file. Each Day corresponds to a date, if the date contains an event. Eventless dates are skipped.</returns>
    public static List<Day> ParseToDays(XDocument scheduleXml)
    {
        var teachersDict = GetScheduleTeachers(scheduleXml);
        var locationsDict = GetScheduleLocations(scheduleXml);
        var coursesDict = GetScheduleCourses(scheduleXml);

        List<Day> days = new();

        foreach (var element in scheduleXml.Descendants("schemaPost"))
        {
            var currentEvent = XmlToEvent(element, teachersDict, locationsDict, coursesDict);

            if (days.Count == 0 || currentEvent.TimeStart.Date != days.Last().Date)
            {
                days.Add(new Day(currentEvent.TimeStart.DayOfWeek.ToString(), currentEvent.TimeStart.Date, new List<Event> { currentEvent }));
                continue;
            }

            days.Last().Events.Add(currentEvent);
        }

        return days;
    }


    /// <summary>
    /// Parse a given <see cref="XElement"/> with Kronox's "schemaPost" format into a full <see cref="Event"/> object.
    /// <para>
    /// Parses the XML element multiple times for different information.
    /// This function does NOTHING besides XML parsing and type casting.
    /// </para>
    /// </summary>
    /// <param name="eventElement"></param>
    /// <param name="teachersDict"></param>
    /// <param name="locationsDict"></param>
    /// <param name="coursesDict"></param>
    /// <returns></returns>
    private static Event XmlToEvent(XElement eventElement, IReadOnlyDictionary<string, Teacher> teachersDict, IReadOnlyDictionary<string, Location> locationsDict, Dictionary<string, Course> coursesDict)
    {
        // Parse all needed Event info from the xml document into strings
        var title = eventElement.Element("moment") == null ? "" : eventElement.Element("moment")!.Value;
        var eventType = eventElement.Element("aktivitetsTyp") == null ? "" : eventElement.Element("aktivitetsTyp")!.FirstAttribute!.Value;
        var eventId = eventElement.Element("bokningsId")!.Value;
        var courseId = GetEventCourseId(eventElement);
        var teacherIds = GetEventTeacherIds(eventElement);
        var locationIds = GetEventLocationIds(eventElement);
        var scheduleIds = GetEventScheduleIds(eventElement);
        var timeStartIsoString = eventElement.Element("bokadeDatum")!.Attribute("startDatumTid_iCal")!.Value;
        var timeEndIsoString = eventElement.Element("bokadeDatum")!.Attribute("slutDatumTid_iCal")!.Value;
        var lastModifiedString = eventElement.Element("senastAndradDatum_iCal")!.Value;

        // Parse and translate the gathered info from above into correct types and formats
        var parsedTitle = Regex.Replace(title, "<.*?>", str => "");
        DateTime.TryParseExact(timeStartIsoString, new string[] { "yyyyMMddTHHmmssZ" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeStart);
        DateTime.TryParseExact(timeEndIsoString, new string[] { "yyyyMMddTHHmmssZ" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime timeEnd);
        DateTime.TryParseExact(lastModifiedString, new string[] { "yyyyMMddTHHmmssZ" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime lastModified);
        List<Teacher> teachers = new();
        List<Location> locations = new();

        // Translate the event's teacher ids into teacher objects
        teacherIds.ForEach(id => teachers.Add(teachersDict.GetValueOrDefault(id, Teacher.NotAvailable)));
        // Translate the event's location ids into location objects
        locationIds.ForEach(id => locations.Add(locationsDict.GetValueOrDefault(id, Location.NotAvailable)));

        return new Event(eventId, scheduleIds.ToArray(), parsedTitle, coursesDict.GetValueOrDefault(courseId, Course.NotAvailable), teachers, timeStart, timeEnd, locations, eventType == "A", lastModified);
    }

    /// <summary>
    /// Get collection of all courses needed for a given schedule XML.
    /// <para>
    /// Searches XML for a "forklaringsrader" element, with attribute "typ=UTB_KURSINSTANS_GRUPPER", to find each course element.
    /// </para>
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <returns><see cref="Dictionary{String, Course}"/> containing each coures id mapped to a course object.</returns>
    private static Dictionary<string, Course> GetScheduleCourses(XContainer xmlDoc)
    {
        Dictionary<string, Course> coursesDict = new();

        var courses =
            xmlDoc.Descendants("forklaringsrader")
                  .Where(el => el.Attribute("typ") != null && el.Attribute("typ")!.Value == "UTB_KURSINSTANS_GRUPPER")
                  .Elements();

        // Parse each course's data into a Teacher object and add it to teachersDict
        foreach (var course in courses)
        {
            // Set default values for the remaining attributes if they're not found in the data
            var courseId = course.Elements().Where(el => el.Attribute("rubrik")!.Value == "Id").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var courseName = course.Elements().Where(el => el.Attribute("rubrik")!.Value == "KursNamn_SV").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;

            // If no course ID was parsed, we don't want to add it to the dict as we can't access it
            if (courseId == string.Empty) continue;

            coursesDict.Add(courseId, new Course(courseName, courseId));
        }

        return coursesDict;
    }

    /// <summary>
    /// Get collection of all teachers needed for a given schedule XML.
    /// <para>
    /// Searches XML for a "forklaringsrader" element, with attribute "typ=RESURSER_SIGNATURER", to find each teacher element.
    /// </para>
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <returns><see cref="Dictionary{String, Teacher}"/> containing the id of each teacher mapped to a teacher object.</returns>
    private static Dictionary<string, Teacher> GetScheduleTeachers(XContainer xmlDoc)
    {
        Dictionary<string, Teacher> teachersDict = new();

        // Get all teacher signatures from the XML
        var signatures =
            xmlDoc.Descendants("forklaringsrader")
                    .Where(el => el.Attribute("typ") != null && el.Attribute("typ")!.Value == "RESURSER_SIGNATURER")
                    .Elements();

        // Parse each teacher's data into a Teacher object and add it to teachersDict
        foreach (var signature in signatures)
        {
            // Set default values for the remaining attributes if they're not found in the data
            var teacherId = signature.Elements().Where(el => el.Attribute("rubrik")!.Value == "Id").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var teacherFirstName = signature.Elements().Where(el => el.Attribute("rubrik")!.Value == "ForNamn").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var teacherLastName = signature.Elements().Where(el => el.Attribute("rubrik")!.Value == "EfterNamn").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;

            // If no teacher ID was parsed, we don't want to add it to the dict as we can't access it
            if (teacherId == string.Empty) continue;

            teachersDict.Add(teacherId, new Teacher(teacherId, teacherFirstName, teacherLastName));
        }

        return teachersDict;
    }

    /// <summary>
    /// Get collection of all locations needed for a given schedule XML.
    /// <para>
    /// Searches XML for a "forklaringsrader" element, with attribute "typ=RESURSER_LOKALER", to find each location element.
    /// </para>
    /// </summary>
    /// <param name="xmlDoc"></param>
    /// <returns><see cref="Dictionary{String, Location}"/> containing the id of each location mapped to a location object.</returns>
    private static Dictionary<string, Location> GetScheduleLocations(XContainer xmlDoc)
    {
        Dictionary<string, Location> locationsDict = new();

        var locations =
            xmlDoc.Descendants("forklaringsrader")
                  .Where(el => el.Attribute("typ") != null && el.Attribute("typ")!.Value == "RESURSER_LOKALER")
                  .Elements();

        foreach (XElement location in locations)
        {
            // Set default values for the remaining attributes if they're not found in the data
            var locationId = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Id").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var locationName = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Lokalnamn").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var locationFloor = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Vaning").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var locationMaxSeats = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Antalplatser").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;
            var locationBuilding = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Hus").FirstOrDefault(new XElement("fail", new XText("N/A"))).Value;

            if (locationId == string.Empty) continue;

            var maxSeatConverted = int.TryParse(locationMaxSeats, out var maxSeatsInt);

            locationsDict.Add(locationId, new Location(locationId, locationName, locationBuilding, locationFloor, maxSeatConverted ? maxSeatsInt : 0));
        }

        return locationsDict;
    }

    /// <summary>
    /// Parse event xml element (Kronox's "schemaPost" element) into a list of teacher ids.
    /// </summary>
    /// <param name="eventElement"></param>
    /// <returns><see cref="List{String}"/> with teacher ids.</returns>
    private static List<string> GetEventTeacherIds(XContainer eventElement)
    {
        List<string> teacherIds = new();

        // Get each teacher element on the Event as an enumberable.
        var teacherElements =
            eventElement.Element("resursTrad")!
                        .Elements("resursNod")
                        .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "RESURSER_SIGNATURER")
                        .AsEnumerable();

        foreach (var teacherElement in teacherElements)
        {
            // Skip any teacher entries that don't contain the necessary data
            if (teacherElement.Element("resursId") == null) continue;
            teacherIds.Add(teacherElement.Element("resursId")!.Value);
        }

        return teacherIds;
    }

    /// <summary>
    /// Parse event xml element (Kronox's "schemaPost" element) into a list of location ids.
    /// </summary>
    /// <param name="eventElement"></param>
    /// <returns><see cref="List{String}"/> with location ids.</returns>
    private static List<string> GetEventLocationIds(XContainer eventElement)
    {
        List<string> locationIds = new();

        // Get each teacher element on the Event as an enumberable.
        var locationElements =
            eventElement.Element("resursTrad")!
                        .Elements("resursNod")
                        .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "RESURSER_LOKALER")
                        .AsEnumerable();

        foreach (var locationElement in locationElements)
        {
            // Skipt any teacher entries that don't contain the necessary data
            if (locationElement.Element("resursId") == null) continue;
            locationIds.Add(locationElement.Element("resursId")!.Value);
        }

        return locationIds;
    }

    /// <summary>
    /// Parse event xml element (Kronox's "schemaPost" element) into the course id related to the event.
    /// </summary>
    /// <param name="eventElement"></param>
    /// <returns>The <see cref="string"/> of the event's related course id.</returns>
    private static string GetEventCourseId(XContainer eventElement)
    {
        return eventElement.Element("resursTrad")!
             .Elements("resursNod")
             .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "UTB_KURSINSTANS_GRUPPER")
             .FirstOrDefault(new XElement("fail"))
             .Element("resursId")!
             .Value;
    }

    /// <summary>
    /// Parse event xml element (Kronox's "schemaPost" element) into a list of the schedule ids connected to the event.
    /// </summary>
    /// <param name="eventElement"></param>
    /// <returns><see cref="List{String}"/> with location ids.</returns>
    private static List<string> GetEventScheduleIds(XElement eventElement)
    {
        List<string> scheduleIds = new();

        // Get each teacher element on the Event as an enumberable.
        var scheduleIdElements =
            eventElement.Element("resursTrad")!
                        .Elements("resursNod")
                        .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "UTB_PROGRAMINSTANS_KLASSER")
                        .AsEnumerable();

        foreach (var scheduleIdElement in scheduleIdElements)
        {
            if (scheduleIdElement.Element("resursIdURLEncoded") == null) continue;
            scheduleIds.Add(scheduleIdElement.Element("resursIdURLEncoded")!.Value);
        }

        return scheduleIds;
    }
}
