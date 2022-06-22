using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    /// <summary>
    /// Model to group together Events from Kronox based on which days they are in.
    /// </summary>
    public class Day
    {
        private readonly string _name;
        private readonly DateOnly _date;
        private readonly List<Event> _events;

        public string Name => _name;

        public DateOnly Date => _date;

        public List<Event> Events => _events;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="date"></param>
        /// <param name="events"></param>
        public Day(string name, DateOnly date, List<Event> events)
        {
            this._name = name;
            this._date = date;
            this._events = events;
        }
    }
}
