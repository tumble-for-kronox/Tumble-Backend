using KronoxAPI.Exceptions;
using KronoxAPI.Utilities;
using System.Web;
using TumbleHttpClient;

namespace KronoxAPI.Controller;

public static class BookingController
{
    public static async Task<string> GetResourcesAsync(IKronoxRequestClient client)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        const string endpoint = "resursbokning.jsp";

        var parameters = new Dictionary<string, string>();

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetPersonalBookingsForResourceAsync(IKronoxRequestClient client, string resourceId)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        var date = DateTime.Now;
        const string endpoint = "minaresursbokningar.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["datum"] = $"{date:yy-MM-dd}",
            ["flik"] = resourceId
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> GetResourceAvailabilityAsync(IKronoxRequestClient client, DateTime date, string resourceId)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        const string endpoint = "ajax/ajax_resursbokning.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["op"] = "hamtaBokningar",
            ["datum"] = $"{date:yy-MM-dd}",
            ["flik"] = resourceId
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task BookResourceLocationAsync(IKronoxRequestClient client, DateTime date, string resourceId, string locationId, string timeSlotId, string resourceType)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        const string endpoint = "ajax/ajax_resursbokning.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["op"] = "boka",
            ["datum"] = $"{date:yy-MM-dd}",
            ["flik"] = resourceId,
            ["id"] = locationId,
            ["typ"] = resourceType,
            ["intervall"] = timeSlotId,
            ["moment"] = "Booked via Tumble"
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        var content = await response.Content.ReadAsStringAsync();
        
        if (content != "OK")
        {
            if (content.Equals("Your user do not have permissions to book resources."))
            {
                throw new LoginException("Kronox failed to authorize the user credentials.");
            }

            if (content.Contains("The booking was not saved because of the following colliding resources:"))
            {
                throw new BookingCollisionException("Couldn't book resource.");
            }

            if (content.Equals("You have already created max number of bookings."))
            {
                throw new MaxBookingsException("");
            }

            throw new ParseException($"Something went wrong while parsing or handling the requset to book a resource. Resource details:\n\nschoolUrl: {client.BaseUrl}\ndate: {date:dd-MM-yy}\nresourceId: {resourceId}\nlocationId: {locationId}\nresourceType: {resourceType}\ntimeSlotId: {timeSlotId}");
        }
    }

    public static async Task UnBookResourceLocationAsync(IKronoxRequestClient client, string bookingId)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        const string endpoint = "ajax/ajax_resursbokning.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["op"] = "avboka",
            ["bokningsId"] = bookingId
        };

        var response = await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
        var content = await response.Content.ReadAsStringAsync();
        
        if (!content.Equals("OK"))
        {
            throw content switch
            {
                "Din användare har inte rättigheter att skapa resursbokningar." => new LoginException(
                    "Kronox failed to authorize the user credentials."),
                "Du kan inte radera resursbokningar där du inte är bokare eller deltagare" => new
                    BookingCollisionException(
                        $"Couldn't unbook resource. Resource details:\n\nschoolUrl: {client.BaseUrl}\nbookingId: {bookingId}"),
                _ => new ParseException(
                    $"Something went wrong while parsing or handling the requset to unbook a resource. Resource details:\n\nschoolUrl: {client.BaseUrl}\nbookingId: {bookingId}")
            };
        }
    }

    public static async Task ConfirmResourceBookingAsync(IKronoxRequestClient client, string bookingId, string resourceId)
    {
        await KronoxSessionController.SetSessionEnglishAsync(client);
        const string endpoint = "ajax/ajax_resursbokning.jsp";

        var parameters = new Dictionary<string, string>
        {
            ["op"] = "konfirmera",
            ["flik"] = resourceId,
            ["bokningsId"] = bookingId
        };

        await HttpUtilities.SendRequestAsync(client, HttpMethod.Get, endpoint, parameters);
    }
}
