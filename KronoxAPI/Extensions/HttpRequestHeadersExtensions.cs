using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;

namespace KronoxAPI.Extensions;

public static class HttpRequestHeadersExtensions
{
    public static HttpRequestHeaders AddAll(this HttpRequestHeaders headers, HttpRequestHeaders? newHeaders)
    {
        if (newHeaders == null) return headers;

        foreach (var header in newHeaders)
        {
            headers.Add(header.Key, header.Value);
        }

        return headers;
    }

    public static HttpContentHeaders AddAll(this HttpContentHeaders headers, IEnumerable<KeyValuePair<string, string>>? newHeaders)
    {
        if (newHeaders == null) return headers;

        foreach (var header in newHeaders)
        {
            headers.Add(header.Key, header.Value);
        }

        return headers;
    }
}
