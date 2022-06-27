using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Scheduling
{
    public class Teacher
    {
        private readonly string _id;
        private readonly string _firstName;
        private readonly string _lastName;

        public Teacher(string id, string firstName, string lastName)
        {
            _id = id;
            _firstName = firstName;
            _lastName = lastName;
        }

        /// <summary>
        /// For use as default or in case a teacher is not found, to make sure nothing breaks.
        /// </summary>
        /// <returns><see cref="Teacher"/> wiht all values set as "N/A"</returns>
        public static Teacher NotAvailable()
        {
            return new Teacher("N/A", "N/A", "N/A");
        }

        public override string? ToString()
        {
            return $"{_firstName} {_lastName}";
        }

        public string Id => _id;

        public string FirstName => _firstName;

        public string LastName => _lastName;
    }
}
