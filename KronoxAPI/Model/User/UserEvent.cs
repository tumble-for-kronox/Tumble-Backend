using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.User
{
    /// <summary>
    /// Model for storing and managing UserEvents (e.g. exams) in Kronox's database.
    /// </summary>
    public class UserEvent
    {
        private readonly string id;
        private readonly string title;
        private readonly string description;
        private readonly DateTime date;
        private readonly bool registered;
        private readonly bool supportAvailable;
        private readonly bool supportActive;
        private readonly bool cityChoiceAvailable;
        private readonly string? selectedCityId;

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
        public UserEvent(string id, string title, string description, DateTime date, bool registered, bool supportAvailable, bool supportActive, bool cityChoiceAvailable, string? selectedCityId)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.date = date;
            this.registered = registered;
            this.supportAvailable = supportAvailable;
            this.supportActive = supportActive;
            this.cityChoiceAvailable = cityChoiceAvailable;
            this.selectedCityId = selectedCityId;
        }

        public string Id => id;

        public string Title => title;

        public string Description => description;

        public DateTime Date => date;

        public bool Registered => registered;

        public bool SupportAvailable => supportAvailable;

        public bool SupportActive => supportActive;

        public bool CityChoiceAvailable => cityChoiceAvailable;

        public string? SelectedCityId => selectedCityId;
    }
}
