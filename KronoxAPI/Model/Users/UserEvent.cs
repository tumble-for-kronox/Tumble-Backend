using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Users
{
    /// <summary>
    /// Model for storing and managing UserEvents (e.g. exams) in Kronox's database.
    /// </summary>
    public class UserEvent
    {
        private readonly string _id;
        private readonly string _title;
        private readonly string _type;
        private readonly DateTime _lastSignupDate;
        private readonly DateTime _eventStart;
        private readonly DateTime _eventEnd;

        private readonly bool _registered;
        private readonly bool _supportAvailable;
        private readonly bool _supportActive;
        private readonly bool _cityChoiceAvailable;
        private readonly string? _selectedCityId;

        public UserEvent(string id, string title, string type, DateTime lastSignupDate, DateTime eventStart, DateTime eventEnd, bool registered, bool supportAvailable, bool supportActive, bool cityChoiceAvailable, string? selectedCityId)
        {
            _id = id;
            _title = title;
            _type = type;
            _lastSignupDate = lastSignupDate;
            _eventStart = eventStart;
            _eventEnd = eventEnd;
            _registered = registered;
            _supportAvailable = supportAvailable;
            _supportActive = supportActive;
            _cityChoiceAvailable = cityChoiceAvailable;
            _selectedCityId = selectedCityId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="date"></param>
        /// <param name="registered"></param>
        /// <param name="supportAvailable"></param>
        /// <param name="supportActive"></param>
        /// <param name="cityChoiceAvailable"></param>
        /// <param name="selectedCityId"></param>
        /// <param name="type"></param>


        public string Id => _id;

        public string Title => _title;

        public bool Registered => _registered;

        public bool SupportAvailable => _supportAvailable;

        public bool SupportActive => _supportActive;

        public bool CityChoiceAvailable => _cityChoiceAvailable;

        public string? SelectedCityId => _selectedCityId;

        public string Type => _type;

        public DateTime LastSignupDate => _lastSignupDate;

        public DateTime EventStart => _eventStart;

        public DateTime EventEnd => _eventEnd;
    }
}
