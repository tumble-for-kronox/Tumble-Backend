using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using KronoxAPI.Model.Users;

namespace WebAPIModels.ResponseModels
{
    public class UserEventCollection
    {
        // Use public properties for JSON deserialization
        public List<UpcomingUserEvent> UpcomingEvents { get; set; }

        public List<AvailableUserEvent> RegisteredEvents { get; set; }

        public List<AvailableUserEvent> UnregisteredEvents { get; set; }

        // Parameterless constructor is required for deserialization
        public UserEventCollection() { }

        public UserEventCollection(List<UpcomingUserEvent> upcomingUserEvents, List<AvailableUserEvent> registeredUserEvents, List<AvailableUserEvent> unregisteredUserEvents)
        {
            UpcomingEvents = upcomingUserEvents;
            RegisteredEvents = registeredUserEvents;
            UnregisteredEvents = unregisteredUserEvents;
        }

        public string ToJson() => JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
    }
}
