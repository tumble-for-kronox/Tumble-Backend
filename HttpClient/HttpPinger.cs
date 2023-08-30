using System;
using System.Collections.Generic;
using System.Linq;
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

    public string Ping(string[] urls)
    {
        
    }
}
