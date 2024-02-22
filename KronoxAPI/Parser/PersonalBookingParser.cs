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
    public static IEnumerable<Booking> ParsePersonalBookings(HtmlDocument personalBookingsHtml, string resourceId)
    {
        try
        {
            List<Booking> bookings = new();

            IEnumerable<HtmlNode>? personalBookingsNodes = personalBookingsHtml.DocumentNode.SelectNodes("//div[@id='minabokningar']/div[@id]");
            personalBookingsNodes ??= new List<HtmlNode>();

            foreach (HtmlNode bookingNode in personalBookingsNodes)
            {
                var showConfirmButton = false;
                var showUnBookButton = false;

                IEnumerable<HtmlNode>? buttons = bookingNode.SelectNodes("div[1]/div[1]/a");
                buttons ??= new List<HtmlNode>();

                foreach (var button in buttons)
                {
                    switch (button.InnerText)
                    {
                        case "Confirm":
                            showConfirmButton = true;
                            break;
                        case "Cancel booking":
                            showUnBookButton = true;
                            break;
                    }
                }

                var bookingId = bookingNode.GetAttributeValue("id", "").Trim().Replace("post_", "");
                var date = bookingNode.SelectSingleNode("div[1]/a").InnerText.Trim();
                var combinedTime = bookingNode.SelectSingleNode("div[1]/text()").InnerText.Trim();
                var locationId = bookingNode.SelectSingleNode("div[1]/b").InnerText.Split(",").Last().Trim();

                var confirmationTimeString = bookingNode.SelectSingleNode("div[2]/span")?.InnerText.Replace("Must be confirmed between ", "").Trim();

                var from = DateTime.ParseExact(date + ' ' + combinedTime.Split(" - ")[0], "yy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                var to = DateTime.ParseExact(date + ' ' + combinedTime.Split(" - ")[1], "yy-MM-dd HH:mm", CultureInfo.InvariantCulture);

                DateTime? confirmTo = null;
                DateTime? confirmFrom = null;

                if (confirmationTimeString != null)
                {
                    confirmFrom = DateTime.ParseExact(date + ' ' + confirmationTimeString.Split(" - ")[0], "yy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                    confirmTo = DateTime.ParseExact(date + ' ' + confirmationTimeString.Split(" - ")[1], "yy-MM-dd HH:mm", CultureInfo.InvariantCulture);
                }

                TimeSlot bookingTimeSlot = new(from, to);

                bookings.Add(new Booking(bookingId, resourceId, bookingTimeSlot, locationId, showConfirmButton, showUnBookButton, confirmFrom, confirmTo));
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
