using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using KronoxAPI.Model.Scheduling;
using WebAPIModels.ResponseModels;

public static class CalendarManager
{
	/// <summary>
	/// Folder name of where ICS file will live. String must be escaped if there are any special characters. 
	/// </summary>
	private static readonly string ICSFileFolder = "ICSFiles";

	public static void SetupCalendarSystem()
	{
		Directory.CreateDirectory($"./{ICSFileFolder}");
	}

	/// <summary>
	/// Generates consistent paths for the schedules that it is given
	/// </summary>
	/// <param name="schedule"></param>
	/// <returns>Relative path of ICS file for schedule</returns>
	private static String GetCalendarPath(ScheduleWebModel schedule)
	{
		return $"./{ICSFileFolder}/{schedule.Id}.ics";
	}
	/// <summary>
	/// Helper function to split strings every n characters
	/// </summary>
	private static IEnumerable<String> SplitInParts(this String s, Int32 partLength)
	{
		for (var i = 0; i < s.Length; i += partLength)
			yield return s.Substring(i, Math.Min(partLength, s.Length - i));
	}
	private static string BetterSubString(String s, int startIndex, int length)
	{
		return s.Substring(startIndex, Math.Min(s.Length, length));
	}

	/// <summary>
	/// Generates a valid ICS formatted string to be read by a calendar. 
	/// </summary>
	/// <param name="schedule">Schedule to generate for</param>
	/// <returns>String in the format of a valid ICS file</returns>
	public static String GenerateICSString(ScheduleWebModel schedule)
	{
		String timeFormat = "yyyyMMdd'T'HHmmss";
		String generationTime = DateTime.UtcNow.ToString(timeFormat) + "Z";

		StringBuilder builder = new StringBuilder();
		builder.Append(
$@"BEGIN:VCALENDAR
PRODID:-//tumble.hkr.se//Tumble Calendar//EN
VERSION:2.0
CALSCALE:GREGORIAN
METHOD:PUBLISH
X-WR-CALNAME:{schedule.Id}
X-WR-TIMEZONE:Europe/Stockholm
");

		// Adding events here
		foreach (DayWebModel day in schedule.Days)
		{
			foreach (EventWebModel dEvent in day.Events)
			{
				// NOTE: The format specifies a max line length of 75 so we are splitting on 73 to account for the CR and LF characters

				string split_title = String.Join("\r\n ", SplitInParts("SUMMARY:" + dEvent.Title, 73));

				builder.Append(
@$"BEGIN:VEVENT
DTSTART:{dEvent.From.ToUniversalTime().ToString(timeFormat)}Z
DTEND:{dEvent.To.ToUniversalTime().ToString(timeFormat)}Z
DTSTAMP:{generationTime}
UID:{dEvent.GetHashCode().ToString()}
DESCRIPTION: Teachers: {String.Join(", ", dEvent.Teachers)}
LOCATION:{String.Join(", ", dEvent.Locations)}
SEQUENCE:0
{split_title}
TRANSP:TRANSPARENT
");

				// Google Calendar doesn't seem to support this????
				// if (dEvent.IsSpecial)
				// {
				// 	builder.Append("PRIORITY: 1\r\n");
				// }

				builder.Append(
@$"BEGIN:VALARM
ACTION:DISPLAY
TRIGGER:-PT30M
{BetterSubString("DESCRIPTION:" + dEvent.Title, 0, 73)}
END:VALARM
END:VEVENT
"
);
			}
		}

		builder.Append("END:VCALENDAR\r\n");
		return builder.ToString();
	}

	public static void UpdateCalendarFile(ScheduleWebModel schedule)
	{
		using (StreamWriter sw = File.CreateText(GetCalendarPath(schedule)))
		{
			sw.Write(GenerateICSString(schedule));
		}
	}
}