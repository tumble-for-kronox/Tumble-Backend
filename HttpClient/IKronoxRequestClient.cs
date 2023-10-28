using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TumbleHttpClient;

public interface IKronoxRequestClient
{
    Uri? BaseUrl { get; }
    bool IsAuthenticated { get; }
    CookieContainer CookieContainer { get; }

    Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
}
