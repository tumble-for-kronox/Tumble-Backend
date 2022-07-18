using KronoxAPI.Model.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.Extensions;

public static class UserEventCollectionExtension
{
    public static UserEventCollection? ToWebModel(this Dictionary<string, List<UserEvent>> userEvents)
    {
        if (!userEvents.ContainsKey("upcoming") || !userEvents.ContainsKey("registered") || !userEvents.ContainsKey("unregistered")) return null;

        return new UserEventCollection(userEvents["upcoming"].Cast<UpcomingUserEvent>().ToList(), userEvents["registered"].Cast<AvailableUserEvent>().ToList(), userEvents["unregistered"].Cast<AvailableUserEvent>().ToList());
    }
}
