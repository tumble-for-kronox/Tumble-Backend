using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.User
{
    /// <summary>
    /// Model for fetching and managing user data in Kronox's database. 
    /// </summary>
    public class User
    {
        private readonly string _username;
        private readonly string _password;
        private readonly string _name;
        private readonly string _participatorId;
        private string? _sessionToken;
        private bool _loggedIn = false;

        public User(string username, string password, string name, string participatorId)
        {
            _username = username;
            _password = password;
            _name = name;
            _participatorId = participatorId;
        }

        public string Username => _username;

        public string Password => _password;

        public string Name => _name;

        public string ParticipatorId => _participatorId;

        public string SessionToken { get => _sessionToken; set => _sessionToken = value; }
        public bool LoggedIn { get => _loggedIn; set => _loggedIn = value; }

        /// <summary>
        /// Login the user, using <see cref="Username"/> and <see cref="Password"/>. Stores the resulting session token in <see cref="SessionToken"/>.
        /// </summary>
        public void Login()
        {
            
        }

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
