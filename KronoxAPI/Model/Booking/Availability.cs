using System.Text.Json.Serialization;

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
