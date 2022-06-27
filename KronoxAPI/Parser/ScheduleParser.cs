using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KronoxAPI.Model.Scheduling;

namespace KronoxAPI.Parser
{
    public static class ScheduleParser
    {
        public static Schedule ParseToSchedule(string scheduleId, string scheduleXml)
        {
            XDocument xmlDoc = XDocument.Parse(scheduleXml);

            return new Schedule(scheduleId, new List<Day>());
        }

        public static List<Event> ParseToEvents(string scheduleXml)
        {
            XDocument xmlDoc = XDocument.Parse(scheduleXml);
            Dictionary<string, Teacher> teachersDict = GetScheduleTeachers(xmlDoc);
            Dictionary<string, Location> locationsDict = GetScheduleLocations(xmlDoc);
            Dictionary<string, Course> coursesDict = GetScheduleCourses(xmlDoc);

            List<Event> events = new List<Event>();

            foreach (XElement e in xmlDoc.Descendants("schemaPost"))
            {
                events.Add(XmlToEvent(e, teachersDict, locationsDict, coursesDict));
            }

            return events;
        }

        public static List<Day> ParseToDays(string scheduleXml)
        {
            return new List<Day>();
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
        public static Event XmlToEvent(XElement eventElement, Dictionary<string, Teacher> teachersDict, Dictionary<string, Location> locationsDict, Dictionary<string, Course> coursesDict)
        {
            // Parse all needed Event info from the xml document
            string title = eventElement.Element("moment") == null ? "" : eventElement.Element("moment")!.Value;
            string courseId =
                eventElement.Element("resursTrad")!
                 .Elements("resursNod")
                 .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "UTB_KURSINSTANS_GRUPPER")
                 .First()
                 .Element("resursId")!
                 .Value;
            List<string> teacherIds = GetEventTeacherIds(eventElement);
            List<string> locationIds = GetEventLocationIds(eventElement);
            string timeStartIsoString = eventElement.Element("bokadeDatum")!.Attribute("startDatumTid_iCal")!.Value;
            string timeEndIsoString = eventElement.Element("bokadeDatum")!.Attribute("slutDatumTid_iCal")!.Value;

            // Parse and translate some of the gathered info from above into correct types and formats
            Course course = coursesDict.GetValueOrDefault(courseId, Course.NotAvailable());
            DateTime timeStart = DateTime.ParseExact(timeStartIsoString, new string[] { "yyyyMMddTHHmmssZ" }, CultureInfo.InvariantCulture, DateTimeStyles.None);
            DateTime timeEnd = DateTime.ParseExact(timeEndIsoString, new string[] { "yyyyMMddTHHmmssZ" }, CultureInfo.InvariantCulture, DateTimeStyles.None);
            List<Teacher> teachers = new List<Teacher>();
            List<Location> locations = new List<Location>();
            foreach (string teacherId in teacherIds)
            {
                teachers.Add(teachersDict.GetValueOrDefault(teacherId, Teacher.NotAvailable()));
            }

            foreach (string locationId in locationIds)
            {
                locations.Add(locationsDict.GetValueOrDefault(locationId, Location.NotAvailable()));
            }

            return new Event(title, course, teachers, timeStart, timeEnd, locations);
        }

        /// <summary>
        /// Get collection of all courses needed for a give schedule XML.
        /// <para>
        /// Searches XML for a "forklaringsrader" element, with attribute "typ=UTB_KURSINSTANS_GRUPPER", to find each course element.
        /// </para>
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <returns><see cref="Dictionary{String, Course}"/> containing each coures id mapped to a course object.</returns>
        public static Dictionary<string, Course> GetScheduleCourses(XDocument xmlDoc)
        {
            Dictionary<string, Course> coursesDict = new();

            IEnumerable<XElement> courses =
                xmlDoc.Descendants("forklaringsrader")
                      .Where(el => el.Attribute("typ") != null && el.Attribute("typ")!.Value == "UTB_KURSINSTANS_GRUPPER")
                      .Elements();

            foreach (XElement course in courses)
            {
                string courseId = course.Elements().Where(el => el.Attribute("rubrik")!.Value == "Id").First().Value;
                string courseName = course.Elements().Where(el => el.Attribute("rubrik")!.Value == "KursNamn_SV").First().Value;

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
        public static Dictionary<string, Teacher> GetScheduleTeachers(XDocument xmlDoc)
        {
            Dictionary<string, Teacher> teachersDict = new();

            IEnumerable<XElement> signatures =
                xmlDoc.Descendants("forklaringsrader")
                      .Where(el => el.Attribute("typ") != null && el.Attribute("typ")!.Value == "RESURSER_SIGNATURER")
                      .Elements();

            foreach (XElement signature in signatures)
            {
                string teacherId = signature.Elements().Where(el => el.Attribute("rubrik")!.Value == "Id").First().Value;
                string teacherFirstName = signature.Elements().Where(el => el.Attribute("rubrik")!.Value == "ForNamn").First().Value;
                string teacherLastName = signature.Elements().Where(el => el.Attribute("rubrik")!.Value == "EfterNamn").First().Value;

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
        public static Dictionary<string, Location> GetScheduleLocations(XDocument xmlDoc)
        {
            Dictionary<string, Location> locationsDict = new();

            IEnumerable<XElement> locations =
                xmlDoc.Descendants("forklaringsrader")
                      .Where(el => el.Attribute("typ") != null && el.Attribute("typ")!.Value == "RESURSER_LOKALER")
                      .Elements();

            foreach (XElement location in locations)
            {
                string locationId = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Id").First().Value;
                string locationName = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Lokalnamn").First().Value;
                string locationFloor = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Vaning").First().Value;
                string locationMaxSeats = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Antalplatser").First().Value;
                string locationBuilding = location.Elements().Where(el => el.Attribute("rubrik")!.Value == "Hus").First().Value;

                locationsDict.Add(locationId, new Location(locationId, locationName, locationBuilding, locationFloor, locationMaxSeats));
            }

            return locationsDict;
        }

        /// <summary>
        /// Parse event xml element (Kronox's "schemaPost" element) into a list of teacher ids.
        /// </summary>
        /// <param name="eventElement"></param>
        /// <returns><see cref="List{String}"/> with teacher ids.</returns>
        private static List<string> GetEventTeacherIds(XElement eventElement)
        {
            List<string> teacherIds = new();

            // Get each teacher element on the Event as an enumberable.
            IEnumerable<XElement> teacherElements =
                eventElement.Element("resursTrad")!
                            .Elements("resursNod")
                            .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "RESURSER_SIGNATURER")
                            .AsEnumerable();

            foreach (XElement teacherElement in teacherElements)
            {
                teacherIds.Add(teacherElement.Element("resursId")!.Value);
            }

            return teacherIds;
        }

        /// <summary>
        /// Parse event xml element (Kronox's "schemaPost" element) into a list of location ids.
        /// </summary>
        /// <param name="eventElement"></param>
        /// <returns><see cref="List{String}"/> with location ids.</returns>
        private static List<string> GetEventLocationIds(XElement eventElement)
        {
            List<string> locationIds = new();

            // Get each teacher element on the Event as an enumberable.
            IEnumerable<XElement> locationElements =
                eventElement.Element("resursTrad")!
                            .Elements("resursNod")
                            .Where(el => el.Attribute("resursTypId") != null && el.Attribute("resursTypId")!.Value == "RESURSER_LOKALER")
                            .AsEnumerable();

            foreach (XElement locationElement in locationElements)
            {
                locationIds.Add(locationElement.Element("resursId")!.Value);
            }

            return locationIds;
        }
    }
}
