using KronoxAPI.Controller;
using KronoxAPI.Model.Scheduling;
using System.Xml.Linq;
using System.Xml;
using KronoxAPI.Utilities;
using KronoxAPI.Parser;

// See https://aka.ms/new-console-template for more information

//string htmlResult = KronoxPushController.Login("lasse_koordt_rosenkrans.poulsen0003@stud.hkr.se", "oUPJA4j@iocd$dp", "schema.hkr.se").Result.htmlResult;
string xmlResult = KronoxFetchController.GetSchedule("p.TBSE2+2020+36+100+NML+en", "schema.hkr.se", LangEnum.En, null, null).Result;
List<Event> events = ScheduleParser.ParseToEvents(xmlResult);

//Console.WriteLine(xmlResult.ToString());

foreach (Event e in events)
{
    Console.WriteLine(e.ToString() + "\n\n");
}

Console.WriteLine("COUNT: " + events.Count);
