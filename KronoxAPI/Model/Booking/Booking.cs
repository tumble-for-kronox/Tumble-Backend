using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class Booking
{
    public Booking(string id, string resourceId, TimeSlot timeSlot, string locationId, bool showConfirmButton, bool showUnbookButton, DateTime? confirmationOpen, DateTime? confirmationClosed)
    {
        Id = id;
        ResourceId = resourceId;
        TimeSlot = timeSlot;
        LocationId = locationId;
        ShowConfirmButton = showConfirmButton;
        ShowUnbookButton = showUnbookButton;
        ConfirmationClosed = confirmationClosed;
        ConfirmationOpen = confirmationOpen;
    }

    public string Id { get; }

    public string ResourceId { get; }

    public TimeSlot TimeSlot { get; }

    public string LocationId { get; }

    public bool ShowConfirmButton { get; }

    public bool ShowUnbookButton { get; }

    public DateTime? ConfirmationOpen { get; }

    public DateTime? ConfirmationClosed { get; }
}
