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
    private readonly Availability _availability;
    private readonly string? _locationId;
    private readonly string? _resourceType;
    private readonly string? _timeSlotId;
    private readonly string? _bookedBy;

    public AvailabilitySlot(Availability availability, string? locationId = null, string? resourceType = null, string? timeSlotId = null, string? bookedBy = null)
    {
        _availability = availability;
        _locationId = locationId;
        _resourceType = resourceType;
        _timeSlotId = timeSlotId;
        _bookedBy = bookedBy;
    }

    public Availability Availability => _availability;

    public string? LocationId => _locationId;

    public string? ResourceType => _resourceType;

    public string? TimeSlotId => _timeSlotId;

    public string? BookedBy => _bookedBy;
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Availability
{
    UNAVAILABLE,
    AVAILABLE,
    BOOKED
}
