using DotNext.Collections.Generic;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TumbleBackend.Extensions;
using TumbleHttpClient;
using WebAPIModels.RequestModels;
using WebAPIModels.ResponseModels;
using Utilities.Pair;

namespace TumbleBackend.ActionFilters
{
    public class KronoxUrlFilter : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            HttpClient pingHttpClient = new();
            HttpPinger pinger = new(pingHttpClient);

            bool foundSchoolId = request.Query.TryGetValue("schoolId", out var schoolIdQuery);

            if (!foundSchoolId && request.Path != "/api/schedules/multi")
            {
                context.Result = new BadRequestObjectResult(new Error("Requires schoolId query parameter."));
                return;
            }

            List<SchoolEnum> schoolIds = new();
            if (request.Path == "/api/schedules/multi")
            {
                MultiSchoolSchedules[] multiSchoolSchedules = (context.ActionArguments["schoolSchedules"] as MultiSchoolSchedules[])!;
                schoolIds = multiSchoolSchedules.Select(multiSchoolSchedule => multiSchoolSchedule.SchoolId).ToList();
            } else
            {
                schoolIds.Add((SchoolEnum)int.Parse(schoolIdQuery));
            }

            School?[] schools = schoolIds.Select(id => id.GetSchool()).ToArray() ;

            if (schools.Length == 0 || schools.Any(school => school == null))
            {
                context.Result = new BadRequestObjectResult(new Error("Invalid school value."));
                return;
            }

            try
            {
                Pair<SchoolEnum, Uri>[] workingUrls = await Task.WhenAll(schools.Select(async school => new Pair<SchoolEnum, Uri>(school!.Id, await pinger.Ping(school.Urls))));
                Pair<SchoolEnum, KronoxRequestClient>[] requestClients = workingUrls.Select(keyValue =>
                {
                    KronoxRequestClient client = context.HttpContext.RequestServices.GetService<KronoxRequestClient>()!;
                    client.SetBaseAddress(keyValue.Value);

                    return new Pair<SchoolEnum, KronoxRequestClient>(keyValue.Key, client);
                }).ToArray();

                if (requestClients.Length == 1)
                {
                    context.HttpContext.Items.Add("kronoxReqClient", requestClients[0].Value);
                    context.HttpContext.Items.Add("kronoxReqClientArray", requestClients.AsEnumerable());
                } else
                {
                    context.HttpContext.Items.Add("kronoxReqClientArray", requestClients.AsEnumerable());
                }
            } catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException)
            {
                context.Result = new ObjectResult(StatusCodes.Status500InternalServerError);
                return;
            }

            await next();
        }
    }
}
