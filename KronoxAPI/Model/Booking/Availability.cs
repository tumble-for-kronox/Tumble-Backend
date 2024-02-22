using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class AvailabilitySlot
{
    public AvailabilitySlot(Availability availability, string? locationId = null, string? resourceType = null, string? timeSlotId = null)
    {
        Availability = availability;
        LocationId = locationId;
        ResourceType = resourceType;
        TimeSlotId = timeSlotId;
    }

    public Availability Availability { get; }

    public string? LocationId { get; }

    public string? ResourceType { get; }

    public string? TimeSlotId { get; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Availability
{
    UNAVAILABLE,
    AVAILABLE,
    BOOKED
}
