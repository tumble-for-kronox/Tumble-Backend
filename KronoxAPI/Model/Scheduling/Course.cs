using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    /// <summary>
    /// Model for storing information relevant to a given course on Kronox.
    /// </summary>
    public class Course
    {
        private readonly string _name;
        private readonly string _id;
        private readonly string _color;

        public string Name => _name;

        public string Id => _id;

        public string Color => _color;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="color"></param>
        public Course(String name, String id, String color)
        {
            this._name = name;
            this._id = id;
            this._color = color;
        }
    }
}
