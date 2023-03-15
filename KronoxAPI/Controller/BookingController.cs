using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using KronoxAPI.Exceptions;
using KronoxAPI.Model.Booking;
using KronoxAPI.Model.Scheduling;
using System.Security.AccessControl;

namespace KronoxAPI.Controller;

public static class BookingController
{
    static readonly HttpClientHandler clientHandler;
    static readonly HttpClient client;

    static BookingController()
    {
        clientHandler = new HttpClientHandler();
        client = new HttpClient(clientHandler);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.36");
    }

    public static async Task<string> GetResources(string schoolUrl, string sessionToken)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        Uri uri = new($"https://{schoolUrl}/resursbokning.jsp");

        // Perform web request
        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetPersonalBookingsForResource(string schoolUrl, string resourceId, string sessionToken)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        DateTime date = DateTime.Now;

        Uri uri = new($"https://{schoolUrl}/minaresursbokningar.jsp?datum={date:yy-MM-dd}&flik={resourceId}");

        // Perform web request
        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetResourceAvailability(string schoolUrl, DateTime date, string resourceId, string sessionToken)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        Uri uri = new($"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=hamtaBokningar&datum={date:yy-MM-dd}&flik={resourceId}");

        // Perform web request
        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schoolUrl"></param>
    /// <param name="date"></param>
    /// <param name="resourceId"></param>
    /// <param name="sessionToken"></param>
    /// <param name="locationId"></param>
    /// <param name="timeSlotId"></param>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="MaxBookingsException"></exception>
    /// <exception cref="ParseException"></exception>
    public static async Task BookResourceLocation(string schoolUrl, DateTime date, string resourceId, string sessionToken, string locationId, string timeSlotId, string resourceType)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        Uri uri = new($"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=boka&datum={date:yy-MM-dd}&flik={resourceId}&id={locationId}&typ={resourceType}&intervall={timeSlotId}&moment=Booked via Tumble");

        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);

        string responseBody = await response.Content.ReadAsStringAsync();
        if (responseBody != "OK")
        {
            Console.WriteLine(responseBody);
            if (responseBody == "Din användare har inte rättigheter att skapa resursbokningar.")
            {
                throw new LoginException("Kronox failed to authorize the user credentials.");
            }

            if (responseBody.Contains("The booking was not saved because of the following colliding resources:"))
            {
                throw new BookingCollisionException("Couldn't book resource.");
            }

            if (responseBody == "You have already created max number of bookings.")
            {
                throw new MaxBookingsException("");
            }

            throw new ParseException($"Something went wrong while parsing or handling the requset to book a resource. Resource details:\n\nschoolUrl: {schoolUrl}\ndate: {date:dd-MM-yy}\nresourceId: {resourceId}\nlocationId: {locationId}\nresourceType: {resourceType}\ntimeSlotId: {timeSlotId}");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="schoolUrl"></param>
    /// <param name="sessionToken"></param>
    /// <param name="bookingId"></param>
    /// <returns></returns>
    /// <exception cref="LoginException"></exception>
    /// <exception cref="BookingCollisionException"></exception>
    /// <exception cref="ParseException"></exception>
    public static async Task UnbookResourceLocation(string schoolUrl, string sessionToken, string bookingId)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        Uri uri = new($"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=avboka&bokningsId={bookingId}");

        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);

        string responseBody = await response.Content.ReadAsStringAsync();
        if (responseBody != "OK")
        {
            if (responseBody == "Din användare har inte rättigheter att skapa resursbokningar.")
            {
                throw new LoginException("Kronox failed to authorize the user credentials.");
            }

            if (responseBody == "Du kan inte radera resursbokningar där du inte är bokare eller deltagare")
            {
                throw new BookingCollisionException($"Couldn't unbook resource. Resource details:\n\nschoolUrl: {schoolUrl}\nbookingId: {bookingId}");
            }

            throw new ParseException($"Something went wrong while parsing or handling the requset to unbook a resource. Resource details:\n\nschoolUrl: {schoolUrl}\nbookingId: {bookingId}");
        }
    }

    public static async Task ConfirmResourceBooking(string schoolUrl, string sessionToken, string bookingId, string resourceId)
    {
        KronoxEnglishSession.SetSessionEnglish(schoolUrl, sessionToken);

        Uri uri = new($"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=konfirmera&flik={resourceId}&bokningsId={bookingId}");

        using var request = new HttpRequestMessage(new HttpMethod("GET"), uri);
        request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");
        HttpResponseMessage response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
    }
}
