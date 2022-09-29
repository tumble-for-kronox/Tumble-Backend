using KronoxAPI.Model.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.RequestModels;

public class ConfirmBookingRequest
{
    public ConfirmBookingRequest(string resourceId, string bookingId)
    {
        ResourceId = resourceId;
        BookingId = bookingId;
    }

    public ConfirmBookingRequest() { }

    public string ResourceId { get; set; }
    public string BookingId { get; set; }
}
