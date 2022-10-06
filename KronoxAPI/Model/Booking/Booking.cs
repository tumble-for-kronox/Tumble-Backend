using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class Booking
{
    private readonly string _id;
    private readonly string _resourceId;
    private readonly TimeSlot _timeSlot;
    private readonly string _locationId;
    private readonly bool _showConfirmButton;
    private readonly bool _showUnbookButton;
    private readonly DateTime? _confirmationOpen;
    private readonly DateTime? _confirmationClosed;

    public Booking(string id, string resourceId, TimeSlot timeSlot, string locationId, bool showConfirmButton, bool showUnbookButton, DateTime? confirmationOpen, DateTime? confirmationClosed)
    {
        _id = id;
        _resourceId = resourceId;
        _timeSlot = timeSlot;
        _locationId = locationId;
        _showConfirmButton = showConfirmButton;
        _showUnbookButton = showUnbookButton;
        _confirmationClosed = confirmationClosed;
        _confirmationOpen = confirmationOpen;
    }

    public string Id => _id;

    public string ResourceId => _resourceId;

    public TimeSlot TimeSlot => _timeSlot;

    public string LocationId => _locationId;

    public bool ShowConfirmButton => _showConfirmButton;

    public bool ShowUnbookButton => _showUnbookButton;

    public DateTime? ConfirmationOpen => _confirmationOpen;

    public DateTime? ConfirmationClosed => _confirmationClosed;
}
