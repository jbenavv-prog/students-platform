using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using StudentsPlatform.Api.IntegrationTests.Infrastructure;

namespace StudentsPlatform.Api.IntegrationTests;

public sealed class CatalogEndpointsTests
{
    [Fact]
    public async Task GetCatalogEndpoints_ShouldRequireAuthentication()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();

        var subjectsResponse = await client.GetAsync("/api/subjects");
        var professorsResponse = await client.GetAsync("/api/professors");

        Assert.Equal(HttpStatusCode.Unauthorized, subjectsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, professorsResponse.StatusCode);
    }

    [Fact]
    public async Task GetCatalogEndpoints_ShouldReturnSeededSubjectsAndProfessors_WhenAuthenticated()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();

        await AuthenticateAsAdminAsync(client);

        var subjectsResponse = await client.GetAsync("/api/subjects");
        var professorsResponse = await client.GetAsync("/api/professors");

        Assert.Equal(HttpStatusCode.OK, subjectsResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, professorsResponse.StatusCode);

        var subjects = await subjectsResponse.Content.ReadFromJsonAsync<List<SubjectResponse>>();
        var professors = await professorsResponse.Content.ReadFromJsonAsync<List<ProfessorResponse>>();

        Assert.NotNull(subjects);
        Assert.NotNull(professors);
        Assert.Equal(10, subjects.Count);
        Assert.Equal(5, professors.Count);
        Assert.All(subjects, subject => Assert.Equal(3, subject.Credits));
        Assert.Equal(5, subjects.Select(subject => subject.ProfessorId).Distinct().Count());
    }

    private static async Task AuthenticateAsAdminAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin@students.local", "Admin1234!"));
        response.EnsureSuccessStatusCode();

        var session = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session!.AccessToken);
    }

    private sealed record LoginRequest(string Email, string Password);

    private sealed record LoginResponse(string AccessToken);

    private sealed record SubjectResponse(Guid Id, string Name, int Credits, Guid ProfessorId, string ProfessorName);

    private sealed record ProfessorResponse(Guid Id, string FullName);
}
