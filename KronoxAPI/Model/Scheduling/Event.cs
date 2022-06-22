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
        private readonly DateTime _timeStart;
        private readonly DateTime _timeEnd;
        private readonly string _location;
        private readonly string _signature;

        public string Title => _title;

        public Course Course => _course;

        public DateTime TimeStart => _timeStart;

        public DateTime TimeEnd => _timeEnd;

        public string Location => _location;

        public string Signature => _signature;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        /// <param name="course"></param>
        /// <param name="timeStart"></param>
        /// <param name="timeEnd"></param>
        /// <param name="location"></param>
        /// <param name="signature"></param>
        public Event(string title, Course course, DateTime timeStart, DateTime timeEnd, string location, string signature)
        {
            this._title = title;
            this._course = course;
            this._timeStart = timeStart;
            this._timeEnd = timeEnd;
            this._location = location;
            this._signature = signature;
        }
    }
}
