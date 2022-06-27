using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    public class Location
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _building;
        private readonly string _floor;
        private readonly string _maxSeats;

        public Location(string id, string name, string building, string floor, string maxSeats)
        {
            _id = id;
            _name = name;
            _building = building;
            _floor = floor;
            _maxSeats = maxSeats;
        }

        /// <summary>
        /// For use as default or in case a location is not found, to make sure nothing breaks.
        /// </summary>
        /// <returns><see cref="Location"/> wiht all values set as "N/A"</returns>
        public static Location NotAvailable()
        {
            return new Location("N/A", "N/A", "N/A", "N/A", "N/A");
        }

        public override string? ToString()
        {
            return $"{_id}";
        }

        public string Id => _id;

        public string Name => _name;

        public string Building => _building;

        public string Floor => _floor;

        public string MaxSeats => _maxSeats;
    }
}
