using DotNext.Collections.Generic;
using KronoxAPI.Model.Schools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TumbleBackend.Extensions;
using TumbleHttpClient;
using WebAPIModels.RequestModels;
using WebAPIModels.ResponseModels;
using Utilities.Pair;
using TumbleBackend.StringConstants;
using TumbleHttpClient.Exceptions;
using System.Net;
using WebAPIModels.MiscModels;

namespace TumbleBackend.ActionFilters
{
    public class KronoxUrlFilter : ActionFilterAttribute
    {
        readonly string multiSchedulePath = "/api/schedules/multi";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var services = context.HttpContext.RequestServices;
            var config = services.GetService<IConfiguration>();
            var request = context.HttpContext.Request;
            HttpClient pingHttpClient = new();
            HttpPinger pinger = new(pingHttpClient);

            bool foundSchoolId = request.Query.TryGetValue("schoolId", out var schoolIdQuery);

            if (!foundSchoolId && request.Path != multiSchedulePath)
            {
                context.Result = new BadRequestObjectResult(new Error("Requires schoolId query parameter."));
                return;
            }

            List<SchoolEnum> schoolIds = new();
            if (request.Path == multiSchedulePath)
            {
                MultiSchoolSchedules[] multiSchoolSchedules = (context.ActionArguments["schoolSchedules"] as MultiSchoolSchedules[])!;
                schoolIds = multiSchoolSchedules.Select(multiSchoolSchedule => multiSchoolSchedule.SchoolId).ToList();
            }
            else
            {
                schoolIds.Add((SchoolEnum)int.Parse(schoolIdQuery));
            }

            School?[] schools = schoolIds.Select(id => id.GetSchool()).ToArray();

            if (schools.Length == 0 || schools.Any(school => school == null))
            {
                context.Result = new BadRequestObjectResult(new Error("Invalid school value."));
                return;
            }

            try
            {
                SessionDetails? sessionDetails = null;
                bool sessionDetailsFound = context.HttpContext.Request.Headers.TryGetValue("X-session-token", out var sessionDetailsJson);
                if (sessionDetailsFound)
                {
                    sessionDetails = SessionDetails.FromJson(sessionDetailsJson);
                }
                Pair<SchoolEnum, Uri>[] workingUrls = await GetWorkingUrls(pinger, schools!, sessionDetails?.SessionLocation);

                Pair<SchoolEnum, KronoxRequestClient>[] requestClients = GetRequestClients(services, workingUrls);

                if (requestClients.Length == 1)
                    context.HttpContext.Items.Add(KronoxReqClientKeys.SingleClient, requestClients[0].Value);

                context.HttpContext.Items.Add(KronoxReqClientKeys.MultiClient, requestClients.AsEnumerable());
            }
            catch (NoValidUrlException ex)
            {
                context.Result = new ObjectResult(new Error("No Kronox connections are available right now."))
                {
                    StatusCode = (int)HttpStatusCode.GatewayTimeout
                };
                return;
            }

            await next();
        }

        private async Task<Pair<SchoolEnum, Uri>[]> GetWorkingUrls(HttpPinger pinger, School[] schools, string? priorityUrl = null)
        {
            List<Pair<SchoolEnum, Uri>> workingUrls = new();

            foreach (var school in schools)
            {
                Uri functionalUrl = await pinger.PingAsync(school.Urls, priorityUrl);

                workingUrls.Add(new Pair<SchoolEnum, Uri>(school.Id, functionalUrl));
            }

            return workingUrls.ToArray();
        }

        private Pair<SchoolEnum, KronoxRequestClient>[] GetRequestClients(IServiceProvider services, Pair<SchoolEnum, Uri>[] urls)
        {
            return urls.Select(keyValue =>
            {
                KronoxRequestClient client = services.GetService<KronoxRequestClient>()!;
                client.SetBaseAddress(keyValue.Value);

                return new Pair<SchoolEnum, KronoxRequestClient>(keyValue.Key, client);
            }).ToArray();
        }
    }
}
