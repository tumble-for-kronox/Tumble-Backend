using HtmlAgilityPack;
using KronoxAPI.Controller;
using KronoxAPI.Exceptions;
using KronoxAPI.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KronoxAPI.Model.Booking;

public class Resource
{
    private readonly string _id;
    private readonly string _name;
    private List<TimeSlot>? _timeSlots;
    private DateTime? _date;
    private List<string>? _locationIds;
    private Dictionary<string, Dictionary<int, AvailabilitySlot>>? _availabilities;

    public Resource(string id, string name)
    {
        _id = id;
        _name = name;
    }

    public string Id => _id;

    public string Name => _name;

    public List<TimeSlot>? TimeSlots { get => _timeSlots; set => _timeSlots = value; }
    public DateTime? Date { get => _date; set => _date = value; }
    public List<string>? LocationIds { get => _locationIds; set => _locationIds = value; }
    public Dictionary<string, Dictionary<int, AvailabilitySlot>>? Availabilities { get => _availabilities; set => _availabilities = value; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schoolUrl"></param>
    /// <param name="sessionToken"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    /// <exception cref="ResourceInavailableException"></exception>
    public async Task<Resource> FetchData(string schoolUrl, string sessionToken, DateTime? date = null)
    {
        date ??= DateTime.Now;

        if (date!.Value.DayOfWeek == DayOfWeek.Saturday || date!.Value.DayOfWeek == DayOfWeek.Sunday)
            throw new ResourceInavailableException("The resource you are attempting to access are not available on Saturdays and Sundays");

        string resourceAvailabilityHtml = await BookingController.GetResourceAvailability(schoolUrl, date.Value, Id, sessionToken);

        HtmlDocument doc = new();
        doc.LoadHtml(resourceAvailabilityHtml);

        this.ParseResourceAvailability(doc, DateOnly.FromDateTime(date.Value));

        return this;
    }
}
