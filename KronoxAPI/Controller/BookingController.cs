using KronoxAPI.Exceptions;
using KronoxAPI.Utilities;
using System.Web;
using TumbleHttpClient;

namespace KronoxAPI.Controller;

public static class BookingController
{
    static readonly MultiRequest client;

    static BookingController()
    {
        client = new MultiRequest();
    }

    public static async Task<string> GetResources(IKronoxRequestClient client)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        string endpoint = "resursbokning.jsp";

        HttpRequestMessage request = new(new HttpMethod("GET"), endpoint);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetPersonalBookingsForResource(IKronoxRequestClient client, string resourceId)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        DateTime date = DateTime.Now;
        string endpoint = "minaresursbokningar.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["datum"] = $"{date:yy-MM-dd}";
        query["flik"] = resourceId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetResourceAvailability(IKronoxRequestClient client, DateTime date, string resourceId)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        string endpoint = "ajax/ajax_resursbokning.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "hamtaBokningar";
        query["datum"] = $"{date:yy-MM-dd}";
        query["flik"] = resourceId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
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
    public static async Task BookResourceLocation(IKronoxRequestClient client, DateTime date, string resourceId, string locationId, string timeSlotId, string resourceType)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        string endpoint = "ajax/ajax_resursbokning.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "boka";
        query["datum"] = $"{date:yy-MM-dd}";
        query["flik"] = resourceId;
        query["id"] = locationId;
        query["typ"] = resourceType;
        query["intervall"] = timeSlotId;
        query["moment"] = "Booked via Tumble";

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);

        string responseBody = await response.Content.ReadAsStringAsync();
        if (responseBody != "OK")
        {
            Console.WriteLine(responseBody);
            if (responseBody == "Your user do not have permissions to book resources.")
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

            throw new ParseException($"Something went wrong while parsing or handling the requset to book a resource. Resource details:\n\nschoolUrl: {client.BaseUrl}\ndate: {date:dd-MM-yy}\nresourceId: {resourceId}\nlocationId: {locationId}\nresourceType: {resourceType}\ntimeSlotId: {timeSlotId}");
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
    public static async Task UnbookResourceLocation(IKronoxRequestClient client, string bookingId)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        string endpoint = "ajax/ajax_resursbokning.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "avboka";
        query["bokningsId"] = bookingId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
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
                throw new BookingCollisionException($"Couldn't unbook resource. Resource details:\n\nschoolUrl: {client.BaseUrl}\nbookingId: {bookingId}");
            }

            throw new ParseException($"Something went wrong while parsing or handling the requset to unbook a resource. Resource details:\n\nschoolUrl: {client.BaseUrl}\nbookingId: {bookingId}");
        }
    }

    public static async Task ConfirmResourceBooking(IKronoxRequestClient client, string bookingId, string resourceId)
    {
        await KronoxEnglishSession.SetSessionEnglish(client);

        string endpoint = "ajax/ajax_resursbokning.jsp";

        var query = HttpUtility.ParseQueryString("");
        query["op"] = "konfirmera";
        query["flik"] = resourceId;
        query["bokningsId"] = bookingId;

        string fullPath = $"{endpoint}?{query}";

        HttpRequestMessage request = new(new HttpMethod("GET"), fullPath);
        HttpResponseMessage response = await client.SendAsync(request);

        response.EnsureSuccessStatusCode();
    }
}
