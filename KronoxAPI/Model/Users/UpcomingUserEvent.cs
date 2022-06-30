using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Users
{
    public class UpcomingUserEvent : UserEvent
    {
        public UpcomingUserEvent(string title, string type, DateTime lastSignupDate, DateTime eventStart, DateTime eventEnd) : base(title, type, lastSignupDate, eventStart, eventEnd)
        {

        }
    }
}
