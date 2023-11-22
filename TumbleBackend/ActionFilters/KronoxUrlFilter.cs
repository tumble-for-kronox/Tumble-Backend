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
using DatabaseAPI.Interfaces;
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
                int urlCacheTimeout = int.Parse(config![AppSettings.KronoxCacheTTL]);
                Pair<SchoolEnum, Uri>[] workingUrls = await GetWorkingUrls(services.GetService<IDbKronoxCacheService>()!, urlCacheTimeout, pinger, schools!);

                Pair<SchoolEnum, KronoxRequestClient>[] requestClients = GetRequestClients(services, workingUrls);

                if (requestClients.Length == 1)
                    context.HttpContext.Items.Add(KronoxReqClientKeys.SingleClient, requestClients[0].Value);

                context.HttpContext.Items.Add(KronoxReqClientKeys.MultiClient, requestClients.AsEnumerable());
            } catch (NoValidUrlException ex)
            {
                context.Result = new ObjectResult(new Error("No Kronox connections are available right now."))
                {
                    StatusCode = (int)HttpStatusCode.GatewayTimeout
                };
                return;
            }

            await next();
        }

        private async Task<Pair<SchoolEnum, Uri>[]> GetWorkingUrls(IDbKronoxCacheService kronoxCacheService, int urlCacheTimeout, HttpPinger pinger, School[] schools)
        {
            List<Pair<SchoolEnum, Uri>> workingUrls = new();

            foreach (var school in schools)
            {
                Uri? functionalUrl = null;
                var cachedKronox = await kronoxCacheService.GetKronoxCacheAsync(school.Id.ToString());

                if (cachedKronox != null && (DateTime.Now - cachedKronox.Timestamp).TotalMinutes <= urlCacheTimeout)
                {
                    functionalUrl = new Uri(cachedKronox.Url);
                }
                else
                {
                    functionalUrl = await pinger.PingAsync(school.Urls);

                    var newCacheEntry = new KronoxCache(school.Id.ToString(), functionalUrl.ToString(), DateTime.Now, school.Id);
                    await kronoxCacheService.UpsertKronoxCacheAsync(newCacheEntry);
                }

                if (functionalUrl == null)
                {
                    throw new NoValidUrlException();
                }

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
