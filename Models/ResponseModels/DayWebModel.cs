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
        private readonly string _isoString;
        private readonly int _weekNumber;
        private readonly List<EventWebModel> _events;

        public string Name => _name;

        public string Date => _date;

        public string IsoString => _isoString;

        public int WeekNumber => _weekNumber;

        public List<EventWebModel> Events => _events;

        public DayWebModel(string name, string date, string isoString, int weekNumber, List<EventWebModel> events)
        {
            _name = name;
            _date = date;
            _isoString = isoString;
            _weekNumber = weekNumber;
            _events = events;
        }
    }
}
