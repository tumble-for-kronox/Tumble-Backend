using KronoxAPI.Model.Scheduling;
using KronoxAPI.Model.Schools;
using KronoxAPI.Utilities;
using System.Diagnostics;
using static KronoxAPI.Controller.KronoxFetchController;

namespace TumbleBackend.Extensions;

public static class ProgrammeExtension
{
    static readonly HttpClient client = new();

    public static async Task<bool> ScheduleAvailable(this Programme programme, string schoolUrl, string? sessionToken)
    {
		try
		{

            string uri = $"https://{schoolUrl}/setup/jsp/SchemaXML.jsp?startDatum={DateTime.Now.FirstDayOfWeek()}&intervallTyp=m&intervallAntal=6&sprak={LangEnum.Sv}&sokMedAND=true&forklaringar=true&resurser={programme.Id}";

            using var request = new HttpRequestMessage(HttpMethod.Head, uri);
            if (sessionToken != null) request.Headers.Add("Cookie", $"JSESSIONID={sessionToken}");

            using HttpResponseMessage response = await client.SendAsync(request);

            if (response.Content.Headers.ContentLength != null)
            {
                Console.Out.WriteLine(response.Content.Headers.ContentLength);
            }

            return false;
		}
		catch (TimeoutException)
		{
			return true;
		}

    }
}
