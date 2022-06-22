using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    /// <summary>
    /// Model for programme data found through Kronox's database.
    /// </summary>
    public class Programme
    {
        private readonly string _name;
        private readonly string _id;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public Programme(string name, string id)
        {
            _name = name;
            _id = id;
        }

        public string Name => _name;

        public string Id => _id;
    }
}
