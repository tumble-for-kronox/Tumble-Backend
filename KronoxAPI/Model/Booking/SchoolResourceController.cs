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

namespace KronoxAPI.Model.Booking;

public class SchoolResources
{
    private readonly School _school;

    public SchoolResources(School school)
    {
        _school = school;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    public List<Booking> GetUserBookings(string sessionToken)
    {
        string resourcesHtml = BookingController.GetResources(_school.Url, sessionToken).Result;
        HtmlDocument doc = new();
        doc.LoadHtml(resourcesHtml);

        List<Resource> resources = ResourceParser.ParseAllSchoolResources(doc);

        List<Booking> allBookings = new();
        foreach (Resource resource in resources)
        {
            string personalBookingsForResourceHtml = BookingController.GetPersonalBookingsForResource(_school.Url, resource.Id, sessionToken).Result;

            HtmlDocument personalBookingsDoc = new();
            personalBookingsDoc.LoadHtml(personalBookingsForResourceHtml);

            allBookings.AddRange(PersonalBookingParser.ParsePersonalBookings(personalBookingsDoc));
        }

        return allBookings;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <returns></returns>
    public List<Resource> GetResources(string sessionToken)
    {
        string resourcesHtml = BookingController.GetResources(_school.Url, sessionToken).Result;
        HtmlDocument doc = new();
        doc.LoadHtml(resourcesHtml);

        return ResourceParser.ParseAllSchoolResources(doc);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <param name="resourceId"></param>
    /// <param name="date"></param>
    /// <param name="slot"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="MaxBookingsException"></exception>
    /// <exception cref="ParseException"></exception>
    public async Task BookResource(string sessionToken, string resourceId, DateTime date, AvailabilitySlot slot)
    {
        if (slot.Availability != Availability.AVAILABLE)
            throw new BookingCollisionException($"The resource could not be booked because the availability was not AVAILABLE. Availability was: {slot.Availability}");
        if (slot.LocationId == null || slot.ResourceType == null || slot.TimeSlotId == null)
            throw new BookingCollisionException($"The resource could not be booked because either locationId, resourceType, or timeSlotId was null. Values:\n\nlocationId: {slot.LocationId}\nresourceType: {slot.ResourceType}\ntimeSlotId: {slot.TimeSlotId}");

        await BookingController.BookResourceLocation(_school.Url, date, resourceId, sessionToken, slot.LocationId!, slot.TimeSlotId!, slot.ResourceType!);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <param name="bookingId"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="ParseException"></exception>
    public async Task UnbookResource(string sessionToken, string bookingId)
    {
        await BookingController.UnbookResourceLocation(_school.Url, sessionToken, bookingId);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <param name="bookingId"></param>
    /// <param name="resourceId"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="ParseException"></exception>
    public async Task ConfirmResourceBooking(string sessionToken, string bookingId, string resourceId)
    {
        await BookingController.ConfirmResourceBooking(_school.Url, sessionToken, bookingId, resourceId);
    }
}
