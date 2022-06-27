using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    /// <summary>
    /// Model for storing data regarding Events (classes) from Kronox's database.
    /// </summary>
    public class Event
    {
        private readonly string _title;
        private readonly Course _course;
        private readonly List<Teacher> _teachers;
        private readonly DateTime _timeStart;
        private readonly DateTime _timeEnd;
        private readonly List<Location> _locations;

        public string Title => _title;

        public Course Course => _course;

        public DateTime TimeStart => _timeStart;

        public DateTime TimeEnd => _timeEnd;

        public List<Location> Locations => _locations;

        public List<Teacher> Teachers => _teachers;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="course"></param>
        /// <param name="teachers"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="locations"></param>
        public Event(string title, Course course, List<Teacher> teachers, DateTime timeStart, DateTime timeEnd, List<Location> locations)
        {
            _title = title;
            _course = course;
            _timeStart = timeStart;
            _timeEnd = timeEnd;
            _locations = locations;
            _teachers = teachers;
        }

        public override string? ToString()
        {
            return $"Course: {_course.Name}\nTitle: {_title}\nStarts: {_timeStart:yyyy-MM-dd HH:mm}" +
                $"\nEnds: {_timeEnd:yyyy-MM-dd HH:mm}\nTeachers: [{String.Join(", ", _teachers)}]\nLocations: [{String.Join(", ", _locations)}]";
        }
    }
}
