using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace TumbleHttpClient;

public class HttpPinger
{
    private readonly HttpClient _httpClient;

    public HttpPinger(HttpClient client, double timeout = 5)
    {
        client.Timeout = TimeSpan.FromSeconds(timeout);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
        this._httpClient = client;
    }

    async public  Result<string, ErrorCode> Ping(string[] urls)
    {
        Task<HttpResponseMessage>[] requests = urls.Select(url =>  _httpClient.GetAsync(url)).ToArray();

        Task<HttpResponseMessage> completedTask = await Task.WhenAny(requests);

        HttpResponseMessage response = await completedTask;

        if (response.StatusCode != HttpStatusCode.OK)
        {

        }
    }
}
