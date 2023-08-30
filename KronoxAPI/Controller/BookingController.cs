using KronoxAPI.Exceptions;
using KronoxAPI.Utilities;
using System.Net.Http.Headers;

namespace KronoxAPI.Controller;

public static class BookingController
{
    static readonly MultiRequest client;

    static BookingController()
    {
        client = new MultiRequest();
    }

    public static async Task<string> GetResources(string[] schoolUrls, string sessionToken)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        };

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/resursbokning.jsp").ToArray();

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetPersonalBookingsForResource(string[] schoolUrls, string resourceId, string sessionToken)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        };

        DateTime date = DateTime.Now;

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/minaresursbokningar.jsp?datum={date:yy-MM-dd}&flik={resourceId}").ToArray();

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetResourceAvailability(string[] schoolUrls, DateTime date, string resourceId, string sessionToken)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        };

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=hamtaBokningar&datum={date:yy-MM-dd}&flik={resourceId}").ToArray();

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);
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
    public static async Task BookResourceLocation(string[] schoolUrls, DateTime date, string resourceId, string sessionToken, string locationId, string timeSlotId, string resourceType)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        };

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=boka&datum={date:yy-MM-dd}&flik={resourceId}&id={locationId}&typ={resourceType}&intervall={timeSlotId}&moment=Booked via Tumble").ToArray();

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);

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

            throw new ParseException($"Something went wrong while parsing or handling the requset to book a resource. Resource details:\n\nschoolUrl: {schoolUrls}\ndate: {date:dd-MM-yy}\nresourceId: {resourceId}\nlocationId: {locationId}\nresourceType: {resourceType}\ntimeSlotId: {timeSlotId}");
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
    public static async Task UnbookResourceLocation(string[] schoolUrls, string sessionToken, string bookingId)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        };

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=avboka&bokningsId={bookingId}").ToArray();

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);

        string responseBody = await response.Content.ReadAsStringAsync();
        if (responseBody != "OK")
        {
            if (responseBody == "Din användare har inte rättigheter att skapa resursbokningar.")
            {
                throw new LoginException("Kronox failed to authorize the user credentials.");
            }

            if (responseBody == "Du kan inte radera resursbokningar där du inte är bokare eller deltagare")
            {
                throw new BookingCollisionException($"Couldn't unbook resource. Resource details:\n\nschoolUrl: {schoolUrls}\nbookingId: {bookingId}");
            }

            throw new ParseException($"Something went wrong while parsing or handling the requset to unbook a resource. Resource details:\n\nschoolUrl: {schoolUrls}\nbookingId: {bookingId}");
        }
    }

    public static async Task ConfirmResourceBooking(string[] schoolUrls, string sessionToken, string bookingId, string resourceId)
    {
        async Task setSessionEnglish(int index)
        {
            await KronoxEnglishSession.SetSessionEnglish(schoolUrls[index], sessionToken);
        };

        string[] uris = schoolUrls.Select(schoolUrl => $"https://{schoolUrl}/ajax/ajax_resursbokning.jsp?op=konfirmera&flik={resourceId}&bokningsId={bookingId}").ToArray();

        Dictionary<string, string> requestHeaders = new()
        {
            { "Cookie", $"JSESSIONID={sessionToken}" }
        };

        HttpResponseMessage response = await client.SendAsync(uris, new("GET"), requestHeaders: requestHeaders, setSessionEnglish: setSessionEnglish);

        response.EnsureSuccessStatusCode();
    }
}
