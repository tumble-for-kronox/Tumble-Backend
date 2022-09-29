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
    private readonly DateTime? _confirmationOpen;
    private readonly DateTime? _confirmationClosed;

    public Booking(string id, TimeSlot timeSlot, string locationId, DateTime? confirmationOpen, DateTime? confirmationClosed)
    {
        _id = id;
        _timeSlot = timeSlot;
        _locationId = locationId;
        _confirmationClosed = confirmationClosed;
        _confirmationOpen = confirmationOpen;
    }

    public string Id => _id;

    public TimeSlot TimeSlot => _timeSlot;

    public string LocationId => _locationId;

    public DateTime? ConfirmationOpen => _confirmationOpen;

    public DateTime? ConfirmationClosed => _confirmationClosed;
}
