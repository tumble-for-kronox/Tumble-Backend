using HtmlAgilityPack;
using KronoxAPI.Controller;
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
    public void BookResource(string sessionToken, string resourceId, DateTime date, AvailabilitySlot slot)
    {
        BookingController.BookResourceLocation(_school.Url, date, resourceId, sessionToken, slot.LocationId!, slot.TimeSlotId!, slot.ResourceType!);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sessionToken"></param>
    /// <param name="bookingId"></param>
    public void UnbookResource(string sessionToken, string bookingId)
    {
        BookingController.UnbookResourceLocation(_school.Url, sessionToken, bookingId);
    }
}
