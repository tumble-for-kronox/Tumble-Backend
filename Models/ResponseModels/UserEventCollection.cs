using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using KronoxAPI.Model.Users;

namespace WebAPIModels.ResponseModels;

public class UserEventCollection
{
    private readonly List<UpcomingUserEvent> _upcomingUserEvents;
    private readonly List<AvailableUserEvent> _registeredUserEvents;
    private readonly List<AvailableUserEvent> _unregisteredUserEvents;

    public List<UpcomingUserEvent> UpcomingUserEvents => _upcomingUserEvents;

    public List<AvailableUserEvent> RegisteredUserEvents => _registeredUserEvents;

    public List<AvailableUserEvent> UnregisteredUserEvents => _unregisteredUserEvents;

    public UserEventCollection(List<UpcomingUserEvent> upcomingUserEvents, List<AvailableUserEvent> registeredUserEvents, List<AvailableUserEvent> unregisteredUserEvents)
    {
        _upcomingUserEvents = upcomingUserEvents;
        _registeredUserEvents = registeredUserEvents;
        _unregisteredUserEvents = unregisteredUserEvents;
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
