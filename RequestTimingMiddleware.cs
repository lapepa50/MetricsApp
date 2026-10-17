using System.Diagnostics;

public sealed record RequestMetric(string Path, double DurationMilliseconds);

public sealed class RequestTimingMiddleware
{
    private static readonly List<RequestMetric> LastRequests = [];
    private static readonly object SyncLock = new();

    private readonly RequestDelegate _next;

    public RequestTimingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            var metric = new RequestMetric(context.Request.Path.ToString(), stopwatch.Elapsed.TotalMilliseconds);

            Console.WriteLine($"Request {context.Request.Method} {metric.Path} took {metric.DurationMilliseconds:F2} ms");

            lock (SyncLock)
            {
                LastRequests.Add(metric);

                if (LastRequests.Count > 20)
                {
                    LastRequests.RemoveAt(0);
                }
            }
        }
    }

    public static RequestMetric[] GetLastRequests()
    {
        lock (SyncLock)
        {
            return [.. LastRequests];
        }
    }
}