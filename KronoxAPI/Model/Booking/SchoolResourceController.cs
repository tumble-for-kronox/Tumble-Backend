using HtmlAgilityPack;
using KronoxAPI.Controller;
using KronoxAPI.Exceptions;
using KronoxAPI.Model.Schools;
using KronoxAPI.Parser;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TumbleHttpClient;

namespace KronoxAPI.Model.Booking;

public class SchoolResources
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    public async Task<List<Booking>> GetUserBookings(IKronoxRequestClient client)
    {
        string resourcesHtml = await BookingController.GetResources(client);
        HtmlDocument doc = new();
        doc.LoadHtml(resourcesHtml);

        List<Resource> resources = ResourceParser.ParseAllSchoolResources(doc);

        List<Booking> allBookings = new();
        foreach (Resource resource in resources)
        {
            string personalBookingsForResourceHtml = await BookingController.GetPersonalBookingsForResource(client, resource.Id);

            HtmlDocument personalBookingsDoc = new();
            personalBookingsDoc.LoadHtml(personalBookingsForResourceHtml);

            allBookings.AddRange(PersonalBookingParser.ParsePersonalBookings(personalBookingsDoc, resource.Id));
        }

        return allBookings;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    /// <exception cref="ParseException"></exception>
    /// <exception cref="LoginException"></exception>
    public async Task<List<Resource>> GetResources(IKronoxRequestClient client)
    {
        string resourcesHtml = await BookingController.GetResources(client);
        HtmlDocument doc = new();
        doc.LoadHtml(resourcesHtml);

        return ResourceParser.ParseAllSchoolResources(doc);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="resourceId"></param>
    /// <param name="date"></param>
    /// <param name="slot"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="MaxBookingsException"></exception>
    /// <exception cref="ParseException"></exception>
    public async Task BookResource(IKronoxRequestClient client, string resourceId, DateTime date, AvailabilitySlot slot)
    {
        if (slot.Availability != Availability.AVAILABLE)
            throw new BookingCollisionException($"The resource could not be booked because the availability was not AVAILABLE. Availability was: {slot.Availability}");
        if (slot.LocationId == null || slot.ResourceType == null || slot.TimeSlotId == null)
            throw new BookingCollisionException($"The resource could not be booked because either locationId, resourceType, or timeSlotId was null. Values:\n\nlocationId: {slot.LocationId}\nresourceType: {slot.ResourceType}\ntimeSlotId: {slot.TimeSlotId}");

        await BookingController.BookResourceLocation(client, date, resourceId, slot.LocationId!, slot.TimeSlotId!, slot.ResourceType!);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="bookingId"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="ParseException"></exception>
    public async Task UnbookResource(IKronoxRequestClient client, string bookingId)
    {
        await BookingController.UnbookResourceLocation(client, bookingId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="client"></param>
    /// <param name="bookingId"></param>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="ParseException"></exception>
    public async Task ConfirmResourceBooking(IKronoxRequestClient client, string bookingId, string resourceId)
    {
        await BookingController.ConfirmResourceBooking(client, bookingId, resourceId);
    }
}
