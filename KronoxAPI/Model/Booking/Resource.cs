using HtmlAgilityPack;
using KronoxAPI.Controller;
using KronoxAPI.Exceptions;
using KronoxAPI.Parser;
using TumbleHttpClient;

namespace KronoxAPI.Model.Booking;

public class Resource
{
    public Resource(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }

    public string Name { get; }

    public List<TimeSlot>? TimeSlots { get; set; }

    public DateTime? Date { get; set; }

    public List<string>? LocationIds { get; set; }

    public Dictionary<string, Dictionary<int, AvailabilitySlot>>? Availabilities { get; set; }

    public async Task<Resource> FetchData(IKronoxRequestClient client, DateTime? date = null)
    {
        date ??= DateTime.Now;

        if (date!.Value.DayOfWeek == DayOfWeek.Saturday || date!.Value.DayOfWeek == DayOfWeek.Sunday)
            throw new ResourceInavailableException("The resource you are attempting to access are not available on Saturdays and Sundays");

        var resourceAvailabilityHtml = await BookingController.GetResourceAvailabilityAsync(client, date.Value, Id);

        HtmlDocument doc = new();
        doc.LoadHtml(resourceAvailabilityHtml);

        this.ParseResourceAvailability(doc, DateOnly.FromDateTime(date.Value));

        return this;
    }
}
