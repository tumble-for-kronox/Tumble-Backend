using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling;

/// <summary>
/// Model for storing data regarding Events (classes) from Kronox's database.
/// </summary>
public class Event
{
    public string Title { get; }

    public Course Course { get; }

    public DateTime TimeStart { get; }

    public DateTime TimeEnd { get; }

    public List<Location> Locations { get; }

    public List<Teacher> Teachers { get; }

    public string Id { get; }

    public string[] ScheduleIds { get; }

    public bool IsSpecial { get; }

    public DateTime LastModified { get; }

    public Event(string id, string[] scheduleIds, string title, Course course, List<Teacher> teachers, DateTime timeStart, DateTime timeEnd, List<Location> locations, bool isSpecial, DateTime lastModified)
    {
        Id = id;
        ScheduleIds = scheduleIds;
        Title = title;
        Course = course;
        TimeStart = timeStart;
        TimeEnd = timeEnd;
        Locations = locations;
        Teachers = teachers;
        LastModified = lastModified;
        IsSpecial = isSpecial;
    }

    public override string? ToString()
        => $"Id: {Id}\nScheduleId: {ScheduleIds}\nCourse: {Course}\nTitle: {Title}\nStarts: {TimeStart:yyyy-MM-dd HH:mm}" +
           $"\nEnds: {TimeEnd:yyyy-MM-dd HH:mm}\nTeachers: [{String.Join(", ", Teachers)}]\nLocations: [{String.Join(", ", Locations)}]";
}
