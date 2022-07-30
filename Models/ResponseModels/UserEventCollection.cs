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
    private readonly List<UpcomingUserEvent> _upcomingEvents;
    private readonly List<AvailableUserEvent> _registeredEvents;
    private readonly List<AvailableUserEvent> _unregisteredEvents;

    public List<UpcomingUserEvent> UpcomingEvents => _upcomingEvents;

    public List<AvailableUserEvent> RegisteredEvents => _registeredEvents;

    public List<AvailableUserEvent> UnregisteredEvents => _unregisteredEvents;

    public UserEventCollection(List<UpcomingUserEvent> upcomingUserEvents, List<AvailableUserEvent> registeredUserEvents, List<AvailableUserEvent> unregisteredUserEvents)
    {
        _upcomingEvents = upcomingUserEvents;
        _registeredEvents = registeredUserEvents;
        _unregisteredEvents = unregisteredUserEvents;
    }

    public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
}
