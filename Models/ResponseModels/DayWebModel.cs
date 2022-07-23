using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KronoxAPI.Model.Scheduling;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPIModels.ResponseModels
{
    public class DayWebModel
    {
        private readonly string _name;
        private readonly string _date;
        private readonly int _year;
        private readonly int _month;
        private readonly int _dayOfMonth;
        private readonly int _dayOfWeek;
        private readonly int _weekNumber;
        private readonly List<Event> _events;

        public string Name => _name;

        public string Date => _date;

        public int Year => _year;

        public int Month => _month;

        public int DayOfMonth => _dayOfMonth;

        public int DayOfWeek => _dayOfWeek;

        public int WeekNumber => _weekNumber;

        public List<Event> Events => _events;

        public DayWebModel(string name, string date, int year, int month, int dayOfMonth, int dayOfWeek, int weekNumber, List<Event> events)
        {
            _name = name;
            _date = date;
            _year = year;
            _month = month;
            _dayOfMonth = dayOfMonth;
            _dayOfWeek = dayOfWeek;
            _weekNumber = weekNumber;
            _events = events;
        }
    }
}
