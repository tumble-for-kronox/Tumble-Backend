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
    public static async Task<List<Booking>> GetUserBookingsAsync(IKronoxRequestClient client)
    {
        var resourcesHtml = await BookingController.GetResourcesAsync(client);
        HtmlDocument doc = new();
        doc.LoadHtml(resourcesHtml);

        var resources = ResourceParser.ParseAllSchoolResources(doc);

        List<Booking> allBookings = new();
        foreach (var resource in resources)
        {
            var personalBookingsForResourceHtml = await BookingController.GetPersonalBookingsForResourceAsync(client, resource.Id);

            HtmlDocument personalBookingsDoc = new();
            personalBookingsDoc.LoadHtml(personalBookingsForResourceHtml);

            allBookings.AddRange(PersonalBookingParser.ParsePersonalBookings(personalBookingsDoc, resource.Id));
        }

        return allBookings;
    }

    public static async Task<List<Resource>> GetResourcesAsync(IKronoxRequestClient client)
    {
        var resourcesHtml = await BookingController.GetResourcesAsync(client);
        HtmlDocument doc = new();
        doc.LoadHtml(resourcesHtml);

        return ResourceParser.ParseAllSchoolResources(doc);
    }

    public static async Task BookResourceAsync(IKronoxRequestClient client, string resourceId, DateTime date, AvailabilitySlot slot)
    {
        if (slot.Availability != Availability.AVAILABLE)
            throw new BookingCollisionException($"The resource could not be booked because the availability was not AVAILABLE. Availability was: {slot.Availability}");
        if (slot.LocationId == null || slot.ResourceType == null || slot.TimeSlotId == null)
            throw new BookingCollisionException($"The resource could not be booked because either locationId, resourceType, or timeSlotId was null. Values:\n\nlocationId: {slot.LocationId}\nresourceType: {slot.ResourceType}\ntimeSlotId: {slot.TimeSlotId}");

        await BookingController.BookResourceLocationAsync(client, date, resourceId, slot.LocationId!, slot.TimeSlotId!, slot.ResourceType!);
    }

    public static async Task UnBookResourceAsync(IKronoxRequestClient client, string bookingId)
        => await BookingController.UnBookResourceLocationAsync(client, bookingId);

    public static async Task ConfirmResourceBookingAsync(IKronoxRequestClient client, string bookingId, string resourceId)
        => await BookingController.ConfirmResourceBookingAsync(client, bookingId, resourceId);
}
