using System.Diagnostics;

namespace StudentsPlatform.Api.Common.Middleware;

public sealed class RequestObservabilityMiddleware(
    RequestDelegate next,
    ILogger<RequestObservabilityMiddleware> logger)
{
    private const string TraceIdHeaderName = "X-Trace-Id";

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.Request.Headers.TryGetValue(TraceIdHeaderName, out var traceIdHeaderValue)
            && !string.IsNullOrWhiteSpace(traceIdHeaderValue))
        {
            httpContext.TraceIdentifier = traceIdHeaderValue.ToString();
        }

        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers[TraceIdHeaderName] = httpContext.TraceIdentifier;
            return Task.CompletedTask;
        });

        var stopwatch = Stopwatch.StartNew();

        await next(httpContext);

        stopwatch.Stop();

        var logLevel = httpContext.Response.StatusCode >= StatusCodes.Status500InternalServerError
            || stopwatch.ElapsedMilliseconds >= 1_000
            ? LogLevel.Warning
            : LogLevel.Information;

        logger.Log(
            logLevel,
            "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds} ms. TraceId: {TraceId}",
            httpContext.Request.Method,
            httpContext.Request.Path,
            httpContext.Response.StatusCode,
            stopwatch.ElapsedMilliseconds,
            httpContext.TraceIdentifier);
    }
}
