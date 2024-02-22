using HtmlAgilityPack;
using KronoxAPI.Exceptions;
using KronoxAPI.Extensions;
using KronoxAPI.Model.Booking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KronoxAPI.Parser;

public static class ResourceParser
{
    public static List<Resource> ParseAllSchoolResources(HtmlDocument resourcesHtml)
    {
        List<Resource> resources = new();

        try
        {
            IEnumerable<HtmlNode> resourceLiList = resourcesHtml.DocumentNode.SelectSingleNode("(//ul[@class='menu'])[2]").SelectNodes("li");
            foreach (HtmlNode li in resourceLiList)
            {
                // /resursbokning.jsp?flik='resourceId' <- we want that resourceId
                var linkContainingResourceId = li.SelectSingleNode("a").GetAttributeValue("href", string.Empty);
                if (linkContainingResourceId == string.Empty)
                    continue;

                try
                {
                    var resourceId = Regex.Match(linkContainingResourceId, @"flik=(.*)").Groups[1].Value.Trim();
                    var resourceName = li.SelectSingleNode("a/em/b").InnerText.Trim();
                    resources.Add(new Resource(resourceId, resourceName));
                }
                catch (IndexOutOfRangeException e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        catch (NullReferenceException e)
        {
            if (resourcesHtml.SessionExpired())
                throw new LoginException("Kronox rejected the login attempt due to bad credentials or something else on their end.", e);

            throw new ParseException("An error occurred while attempting to parse school resources.", e);
        }

        return resources;
    }

    public static void ParseResourceAvailability(this Resource resource, HtmlDocument resourceAvailableHtml, DateOnly resourceDate)
    {
        try
        {
            resource.Date = resourceDate.ToDateTime(TimeOnly.MinValue);

            // Start by parsing out timeslots
            resource.TimeSlots = ParseResourceTimeSlots(resourceAvailableHtml, resourceDate);

            // Now parse out location ids
            resource.LocationIds = ParseResourceLocationIds(resourceAvailableHtml);

            // Put together location ids, timeslots and their availability
            Dictionary<string, Dictionary<int, AvailabilitySlot>> availability = new();
            for (var i = 0; i < resource.LocationIds.Count; i++)
            {
                availability.Add(resource.LocationIds[i], new Dictionary<int, AvailabilitySlot>());
                for (var j = 0; j < resource.TimeSlots.Count; j++)
                {
                    if (resource.TimeSlots[j].Id == null)
                    {
                        throw new ParseException($"A TimeSlot didn't have an Id which is required for parsing availability. TimeSlot: {resource.TimeSlots[j]}");
                    }

                    var availabilityNode = resourceAvailableHtml.DocumentNode.SelectSingleNode($"//tr[{i + 2}]/td[{j + 2}]");
                    switch (availabilityNode.GetAttributeValue("class", " ").Split(" ")[0])
                    {

                        case "grupprum-passerad":
                            availability[resource.LocationIds[i]].Add(resource.TimeSlots[j].Id!.Value, ParseUnavailable());
                            break;
                        case "grupprum-upptagen":
                            availability[resource.LocationIds[i]].Add(resource.TimeSlots[j].Id!.Value, ParseBooked(availabilityNode));
                            break;
                        case "grupprum-ledig":
                            availability[resource.LocationIds[i]].Add(resource.TimeSlots[j].Id!.Value, ParseAvailable(availabilityNode));
                            break;
                    }
                }
            }

            resource.Availabilities = availability;
        }
        catch (NullReferenceException e)
        {
            throw OnError(resourceAvailableHtml, e);
        }
        catch (ArgumentNullException e)
        {
            throw OnError(resourceAvailableHtml, e);
        }
        catch (IndexOutOfRangeException e)
        {
            throw OnError(resourceAvailableHtml, e);
        }
    }

    private static AvailabilitySlot ParseUnavailable()
    {
        return new AvailabilitySlot(Availability.UNAVAILABLE);
    }

    private static AvailabilitySlot ParseBooked(HtmlNode slotNode)
    {
        return new AvailabilitySlot(Availability.BOOKED);
    }

    private static AvailabilitySlot ParseAvailable(HtmlNode slotNode)
    {
        var onClick = slotNode.SelectSingleNode("a").GetAttributeValue("onclick", string.Empty);
        if (string.IsNullOrEmpty(onClick))
            throw new ParseException("The on click attribute of a supposedly available slot returned empty, can't get necessary info for booking.");

        var regexOnClickMatch = Regex.Match(onClick, @"boka\('(.*?)','(.*?)','(.*?)','(.*?)'\)");
        return new AvailabilitySlot(Availability.AVAILABLE, locationId: regexOnClickMatch.Groups[1].Value, resourceType: regexOnClickMatch.Groups[2].Value, timeSlotId: regexOnClickMatch.Groups[3].Value);
    }

    private static List<TimeSlot> ParseResourceTimeSlots(HtmlDocument resourceHtml, DateOnly resourceDate)
    {
        try
        {
            List<TimeSlot> timeSlots = new();

            var timeRangeStrings = resourceHtml.DocumentNode.SelectNodes("//tr[1]/td/b").Select(e => e.InnerText.Trim());

            foreach (var (timeRange, i) in timeRangeStrings.WithIndex())
            {
                var from = resourceDate.ToDateTime(TimeOnly.ParseExact(timeRange.Split(" - ")[0], "HH:mm", CultureInfo.InvariantCulture));
                var to = resourceDate.ToDateTime(TimeOnly.ParseExact(timeRange.Split(" - ")[1], "HH:mm", CultureInfo.InvariantCulture));

                timeSlots.Add(new TimeSlot(id: i, from: from, to: to));
            }

            return timeSlots;
        }
        catch (NullReferenceException e)
        {
            throw OnError(resourceHtml, e);
        }
        catch (ArgumentNullException e)
        {
            throw OnError(resourceHtml, e);
        }
    }

    private static List<string> ParseResourceLocationIds(HtmlDocument resourceHtml)
    {
        try
        {
            return resourceHtml.DocumentNode.SelectNodes("//tr[position()>1]/td[1]/b/text()").Select(e => e.InnerText.Trim()).ToList();
        }
        catch (NullReferenceException e)
        {
            throw OnError(resourceHtml, e);
        }
        catch (ArgumentNullException e)
        {
            throw OnError(resourceHtml, e);
        }
    }

    private static Exception OnError(HtmlDocument resourceHtml, Exception e)
    {
        var loginNode = resourceHtml.DocumentNode.SelectSingleNode("//div[@id='boka-dialog-login']");

        if (loginNode != null || resourceHtml.DocumentNode.InnerText == "Din användare har inte rättigheter att skapa resursbokningar.")
        {
            throw new LoginException("Kronox rejected the login attempt due to bad credentials or something else on their end.", e);
        }

        throw new ParseException("An error occurred while attempting to parse school resources.", e);
    }
}
