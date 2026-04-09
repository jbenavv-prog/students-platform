using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace StudentsPlatform.Api.Common.HealthChecks;

public static class HealthCheckResponseWriter
{
    public static HealthCheckOptions Create(Predicate<HealthCheckRegistration> predicate)
    {
        return new HealthCheckOptions
        {
            Predicate = registration => predicate(registration),
            ResponseWriter = WriteAsync,
            ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            }
        };
    }

    private static Task WriteAsync(HttpContext httpContext, HealthReport healthReport)
    {
        var response = new
        {
            status = healthReport.Status.ToString(),
            totalDurationMs = healthReport.TotalDuration.TotalMilliseconds,
            traceId = httpContext.TraceIdentifier,
            checks = healthReport.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    durationMs = entry.Value.Duration.TotalMilliseconds
                })
        };

        return httpContext.Response.WriteAsJsonAsync(response);
    }
}
