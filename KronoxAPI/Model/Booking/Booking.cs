using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class Booking
{
    private readonly string _id;
    private readonly TimeSlot _timeSlot;
    private readonly string _locationId;

    public Booking(string id, TimeSlot timeSlot, string locationId)
    {
        _id = id;
        _timeSlot = timeSlot;
        _locationId = locationId;
    }

    public string Id => _id;

    public TimeSlot TimeSlot => _timeSlot;

    public string LocationId => _locationId;
}
