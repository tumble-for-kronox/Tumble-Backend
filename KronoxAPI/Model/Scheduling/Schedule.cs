using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    /// <summary>
    /// Model for storing schedule data fetched from Kronox's database.
    /// </summary>
    public class Schedule
    {
        private readonly DateTime _cachedAt;
        private readonly string _id;
        private readonly List<Day> _days;

        public DateTime CachedAt => _cachedAt;

        public string Id => _id;

        public List<Day> Days => _days;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cachedAt"></param>
        /// <param name="id"></param>
        /// <param name="days"></param>
        public Schedule(DateTime cachedAt, string id, List<Day> days)
        {
            this._cachedAt = cachedAt;
            this._id = id;
            this._days = days;
        }
    }
}
