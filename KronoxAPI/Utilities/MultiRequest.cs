using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using KronoxAPI.Extensions;
using KronoxAPI.Controller;

namespace KronoxAPI.Utilities;

public class MultiRequest
{
    readonly HttpClient client;
    public readonly HttpClientHandler clientHandler;

    public MultiRequest(int timeout = 5)
    {
        clientHandler = new HttpClientHandler();
        client = new HttpClient(clientHandler, false);
        client.Timeout = TimeSpan.FromSeconds(timeout);
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");
    }

    /// <summary>
    /// <para>
    /// Supports sending the same request to multiple URLs one after the other, until one response does not timeout.
    /// </para>
    /// <para>
    /// If all requests time out an <see cref="HttpRequestException"/> is thrown.
    /// </para>
    /// </summary>
    /// <param name="urls"></param>
    /// <param name="content"></param>
    /// <param name="query"></param>
    /// <param name="headers"></param>
    /// <param name="contentHeaders"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<HttpResponseMessage> SendAsyncOLD(string[] urls, HttpMethod method, HttpContent? content = null, NameValueCollection? query = null, HttpRequestHeaders? headers = null, IEnumerable<KeyValuePair<string, string>>? contentHeaders = null, Func<string, Task>? setSessionEnglish = null)
    {
        CancellationTokenSource cts = new();
        CancellationToken ct = cts.Token;
        // here you specify how long you want to wait for task to finish before cancelling
        int timeout = 5000;
        cts.CancelAfter(timeout);

        HttpContent[]? bodyContent = content == null ? null : Enumerable.Repeat(content, urls.Length).ToArray();

        Task<HttpResponseMessage>[] requests = urls.Select((url, index) =>
        {
            if (query != null)
                url = new(url + "?" + query.ToString());

            using HttpRequestMessage request = new(method, url);

            if (bodyContent != null)
            {
                request.Content = bodyContent[index];
                request.Content?.Headers.AddAll(contentHeaders);
            }
            request.Headers.AddAll(headers);

            return client.SendAsync(request, ct);
        }).ToArray();

        Task<HttpResponseMessage> task = await Task.WhenAny(requests);
        HttpResponseMessage response = await task;
        cts.Cancel();

        return response;
    }

    public async Task<HttpResponseMessage> SendAsync(string[] urls, HttpMethod httpMethod, HttpContent? content = null, Dictionary<string, string>? queryParams = null, Dictionary<string, string>? requestHeaders = null, Dictionary<string, string>? contentHeaders = null, Func<int, Task>? setSessionEnglish = null)
    {
        queryParams ??= new();
        requestHeaders ??= new();
        contentHeaders ??= new();

        List<Task<HttpResponseMessage>> tasks = new();

        foreach (var (url, index) in urls.WithIndex())
        {
            if (setSessionEnglish != null)
                await setSessionEnglish(index);

            UriBuilder uriBuilder = new UriBuilder(url);
            if (queryParams != null)
            {
                var query = System.Web.HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var param in queryParams)
                {
                    query[param.Key] = param.Value;
                }
                uriBuilder.Query = query.ToString();
            }

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, uriBuilder.Uri);

            if (content != null)
            {
                request.Content = content;
                foreach (var header in contentHeaders)
                {
                    if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value);
                    }
                    else
                    {
                        request.Content.Headers.Add(header.Key, header.Value);
                    }
                }
            }

            foreach (var header in requestHeaders)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            tasks.Add(client.SendAsync(request));
        }

        Task<HttpResponseMessage> completedTask = await Task.WhenAny(tasks);
        return await completedTask;
    }
}
