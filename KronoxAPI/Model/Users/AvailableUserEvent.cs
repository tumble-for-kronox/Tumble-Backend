using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Users
{
    public class AvailableUserEvent : UserEvent
    {
        private readonly string _id;
        private readonly string? _participatorId;
        private readonly string? _supportId;
        private readonly string _anonymousCode;

        private readonly bool _isRegistered;
        private readonly bool _supportAvailable;

        public AvailableUserEvent(string title, string type, DateTime lastSignupDate, DateTime eventStart, DateTime eventEnd, string id, string? participatorId, string? supportId, string anonymousCode, bool isRegistered, bool supportAvailable) : base(title, type, lastSignupDate, eventStart, eventEnd)
        {
            _id = id;
            _participatorId = participatorId;
            _supportId = supportId;
            _anonymousCode = anonymousCode;
            _supportAvailable = supportAvailable;
            _isRegistered = isRegistered;
        }

        public string Id => _id;

        public string? ParticipatorId => _participatorId;

        public string? SupportId => _supportId;

        public string AnonymousCode => _anonymousCode;

        public bool IsRegistered => _isRegistered;

        public bool SupportAvailable => _supportAvailable;
    }
}
