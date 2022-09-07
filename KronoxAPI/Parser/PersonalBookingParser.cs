using HtmlAgilityPack;
using KronoxAPI.Exceptions;
using KronoxAPI.Extensions;
using KronoxAPI.Model.Booking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Parser;

public static class PersonalBookingParser
{
    public static List<Booking> ParsePersonalBookings(HtmlDocument personalBookingsHtml)
    {
        try
        {
            List<Booking> bookings = new();

            IEnumerable<HtmlNode>? personalBookingsNodes = personalBookingsHtml.DocumentNode.SelectNodes("//div[@id='minabokningar']/div[@id]");
            personalBookingsNodes ??= new List<HtmlNode>();

            foreach (HtmlNode bookingNode in personalBookingsNodes)
            {
                // Get raw data, some will need processing before being built into data object
                string bookingId = bookingNode.GetAttributeValue("id", "").Trim().Replace("post_", "");
                string date = bookingNode.SelectSingleNode("div[1]/a").InnerText.Trim();
                string combinedTime = bookingNode.SelectSingleNode("div[1]/text()").InnerText.Trim();
                string locationId = bookingNode.SelectSingleNode("div[1]/b").InnerText.Split(",").Last().Trim();

                // Start date conversions
                DateTime from = DateTime.ParseExact(date + ' ' + combinedTime.Split(" - ")[0], "yy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                DateTime to = DateTime.ParseExact(date + ' ' + combinedTime.Split(" - ")[1], "yy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                TimeSlot bookingTimeSlot = new(from, to);

                bookings.Add(new(bookingId, bookingTimeSlot, locationId));
            }

            return bookings;
        }
        catch (NullReferenceException e)
        {
            if (personalBookingsHtml.SessionExpired())
                throw new LoginException("Kronox rejected the login attempt due to bad credentials or something else on their end.", e);

            throw new ParseException("An error occurred while attempting to parse school resources.", e);
        }

    }
}
