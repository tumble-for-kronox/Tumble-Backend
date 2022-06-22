using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Schools
{
    /// <summary>
    /// Model for storing data regarding different Kronox schools.
    /// <para>
    /// The model also implements fetching and searching in Kronox's backend, to the relative school.
    /// </para>
    /// </summary>
    public class School
    {
        private readonly string _id;
        private readonly string _name;
        private readonly string _url;
        private readonly bool _loginRequired;

        public string Id => _id;

        public string Name => _name;

        public string Url => _url;

        public bool LoginRequired => _loginRequired;

        /// <summary>
        /// <para>
        /// For basic interaction with the schools in Kronox's database it is recommended
        /// to use <see cref="SchoolFactory"/> for constructing <see cref="School"/> instances.
        /// </para>
        /// <para>
        /// Using this constructor directly may create issues throughout the remainder of the
        /// implemented workflows, but is possible if needed.
        /// </para>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="url"></param>
        /// <param name="loginRequired"></param>
        public School(string id, string name, string url, bool loginRequired)
        {
            this._id = id;
            this._url = url;
            this._loginRequired = loginRequired;
            this._name = name;
        }

        /// <summary>
        /// Fetch a specified schedule from the relative school, constructed through <see cref="SchoolFactory"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public Scheduling.Schedule FetchSchedule(string id, DateTime? startDate)
        {
            return new Scheduling.Schedule(new DateTime(), "ass", new List<Scheduling.Day>());
        }

        public List<Scheduling.Programme> SearchProgrammes(string searchQuery)
        {
            return new List<Scheduling.Programme>();
        }
    }
}
