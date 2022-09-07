using KronoxAPI.Model.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPIModels.RequestModels;

public class BookingRequest
{
    public BookingRequest(string resourceId, DateTime date, AvailabilitySlot slot)
    {
        ResourceId = resourceId;
        Date = date;
        Slot = slot;
    }

    public BookingRequest() { }

    public string ResourceId { get; set; }
    public DateTime Date { get; set; }
    public AvailabilitySlot Slot { get; set; }


}
