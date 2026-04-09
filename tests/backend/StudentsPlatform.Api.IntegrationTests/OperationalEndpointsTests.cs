using System.Net;
using System.Text.Json;
using StudentsPlatform.Api.IntegrationTests.Infrastructure;

namespace StudentsPlatform.Api.IntegrationTests;

public sealed class OperationalEndpointsTests
{
    [Fact]
    public async Task HealthEndpoints_ShouldReportHealthy()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();

        var liveResponse = await client.GetAsync("/health/live");
        var readyResponse = await client.GetAsync("/health/ready");
        var defaultHealthResponse = await client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, liveResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, readyResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, defaultHealthResponse.StatusCode);

        var livePayload = await ReadHealthResponseAsync(liveResponse);
        var readyPayload = await ReadHealthResponseAsync(readyResponse);
        var defaultPayload = await ReadHealthResponseAsync(defaultHealthResponse);

        Assert.Equal("Healthy", livePayload.Status);
        Assert.Equal("Healthy", readyPayload.Status);
        Assert.Equal("Healthy", defaultPayload.Status);
        Assert.Contains("self", livePayload.Checks.Keys);
        Assert.Contains("database", readyPayload.Checks.Keys);
        Assert.Contains("database", defaultPayload.Checks.Keys);
    }

    [Fact]
    public async Task Responses_ShouldIncludeTraceIdHeader()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();

        const string traceId = "trace-observability-test";
        using var request = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        request.Headers.Add("X-Trace-Id", traceId);

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(response.Headers.TryGetValues("X-Trace-Id", out var values));
        Assert.Equal(traceId, values.Single());

        var payload = await ReadHealthResponseAsync(response);
        Assert.Equal(traceId, payload.TraceId);
    }

    private static async Task<HealthResponse> ReadHealthResponseAsync(HttpResponseMessage response)
    {
        using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = jsonDocument.RootElement;

        var checks = root.GetProperty("checks")
            .EnumerateObject()
            .ToDictionary(
                property => property.Name,
                property => property.Value.GetProperty("status").GetString() ?? string.Empty);

        return new HealthResponse(
            root.GetProperty("status").GetString() ?? string.Empty,
            root.GetProperty("traceId").GetString() ?? string.Empty,
            checks);
    }

    private sealed record HealthResponse(
        string Status,
        string TraceId,
        Dictionary<string, string> Checks);
}
