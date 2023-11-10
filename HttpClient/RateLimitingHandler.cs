namespace TumbleHttpClient;

public class RateLimitingHandler : DelegatingHandler
{
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxRequests;
    private readonly TimeSpan _timeSpan;
    private readonly Queue<DateTime> _requestTimestamps;

    public RateLimitingHandler(int maxRequests = 100, TimeSpan? timeSpan = null)
    {
        _maxRequests = maxRequests;
        _timeSpan = timeSpan ?? TimeSpan.FromMinutes(1);
        _requestTimestamps = new Queue<DateTime>();
        _semaphore = new SemaphoreSlim(1, 1);
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            var now = DateTime.UtcNow;
            while (_requestTimestamps.Count > 0 && (now - _requestTimestamps.Peek()) > _timeSpan)
            {
                _requestTimestamps.Dequeue();
            }

            if (_requestTimestamps.Count >= _maxRequests)
            {
                throw new Exception("Rate limit exceeded. Please wait before making more requests.");
            }

            _requestTimestamps.Enqueue(now);
        }
        finally
        {
            _semaphore.Release();
        }

        return await base.SendAsync(request, cancellationToken);
    }
}

