using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Users
{
    /// <summary>
    /// Model for fetching and managing user data in Kronox's database. 
    /// </summary>
    public class User
    {
        private readonly string _name;
        private readonly string _participatorId;
        private string? _sessionToken;
        private bool _loggedIn = false;

        public User(string name, string participatorId, string? sessionToken, bool loggedIn)
        {
            _name = name;
            _participatorId = participatorId;
            _sessionToken = sessionToken;
            _loggedIn = loggedIn;
        }

        public string? Name => _name;
        public string? ParticipatorId => _participatorId;
        public string? SessionToken { get => _sessionToken; set => _sessionToken = value; }
        public bool LoggedIn { get => _loggedIn; set => _loggedIn = value; }

        /// <summary>
        /// Login the user, using <see cref="Username"/> and <see cref="Password"/>.
        /// Updates <see cref="SessionToken"/>, <see cref="LoggedIn"/>, <see cref="Name"/>, and <see cref="ParticipatorId"/> from Kronox's information.
        /// </summary>


        /// <summary>
        /// Fetch a list of <see cref="UserEvent"/> from Kronox's database.
        /// </summary>
        /// <returns>List of events connected to the User.</returns>
        public List<UserEvent> GetUserEvents()
        {
            return new List<UserEvent>();
        }
    }
}
