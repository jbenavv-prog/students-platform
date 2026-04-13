using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using StudentsPlatform.Api.IntegrationTests.Infrastructure;

namespace StudentsPlatform.Api.IntegrationTests;

public sealed class StudentsEndpointsTests
{
    [Fact]
    public async Task CrudFlow_ShouldCreateUpdateListAndDeleteStudent()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var initialSelection = SelectSubjectsFromDistinctProfessors(subjects, 3);

        var createResponse = await client.PostAsJsonAsync("/api/students", new StudentRequest(
            "Alice Johnson",
            "alice@example.com",
            "Software Engineering",
            "Test1234!",
            initialSelection.Select(subject => subject.Id).ToArray()));

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdStudent = await createResponse.Content.ReadFromJsonAsync<StudentDetailResponse>();
        Assert.NotNull(createdStudent);
        Assert.Equal(9, createdStudent.Enrollment!.TotalCredits);

        var list = await client.GetFromJsonAsync<List<StudentListItemResponse>>("/api/students");
        Assert.NotNull(list);
        Assert.Contains(list, item => item.Email == "alice@example.com" && item.SubjectsCount == 3);

        var updatedSelection = SelectSubjectsFromDistinctProfessors(subjects.Skip(1).ToList(), 3);

        var updateResponse = await client.PutAsJsonAsync($"/api/students/{createdStudent.Id}", new StudentRequest(
            "Alice Cooper",
            "alice.cooper@example.com",
            "Computer Science",
            null,
            updatedSelection.Select(subject => subject.Id).ToArray()));

        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);

        var updatedStudent = await updateResponse.Content.ReadFromJsonAsync<StudentDetailResponse>();
        Assert.NotNull(updatedStudent);
        Assert.Equal("Alice Cooper", updatedStudent.FullName);
        Assert.Equal(9, updatedStudent.Enrollment!.TotalCredits);

        var detailResponse = await client.GetAsync($"/api/students/{createdStudent.Id}");
        Assert.Equal(HttpStatusCode.OK, detailResponse.StatusCode);

        var deleteResponse = await client.DeleteAsync($"/api/students/{createdStudent.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var deletedStudentResponse = await client.GetAsync($"/api/students/{createdStudent.Id}");
        Assert.Equal(HttpStatusCode.NotFound, deletedStudentResponse.StatusCode);
    }

    [Fact]
    public async Task CreateStudent_ShouldReject_WhenTwoSubjectsBelongToTheSameProfessor()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var duplicatedProfessorSelection = subjects
            .GroupBy(subject => subject.ProfessorId)
            .First(group => group.Count() >= 2)
            .Take(2)
            .Select(subject => subject.Id)
            .Concat(
                subjects
                    .Where(subject => subject.ProfessorId != subjects
                        .GroupBy(item => item.ProfessorId)
                        .First(group => group.Count() >= 2)
                        .Key)
                    .Take(1)
                    .Select(subject => subject.Id))
            .ToArray();

        var response = await client.PostAsJsonAsync("/api/students", new StudentRequest(
            "Bob Smith",
            "bob@example.com",
            "Data Science",
            "Test1234!",
            duplicatedProfessorSelection));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await ReadProblemAsync(response);
        Assert.Equal(400, problem.Status);
        Assert.Equal("No puedes seleccionar dos materias del mismo profesor.", problem.Detail);
    }

    [Fact]
    public async Task PutStudentSubjects_ShouldReject_WhenMoreThanThreeSubjectsAreSelected()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var createdStudent = await CreateStudentAsync(client, new StudentRequest(
            "Charlie Adams",
            "charlie@example.com",
            "Information Systems",
            "Test1234!",
            SelectSubjectsFromDistinctProfessors(subjects, 3).Select(subject => subject.Id).ToArray()));

        var invalidSelection = SelectSubjectsFromDistinctProfessors(subjects, 4)
            .Select(subject => subject.Id)
            .ToArray();

        var response = await client.PutAsJsonAsync($"/api/students/{createdStudent.Id}/subjects", new SubjectsRequest(invalidSelection));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await ReadProblemAsync(response);
        Assert.Equal(400, problem.Status);
        Assert.Equal("Un estudiante debe seleccionar exactamente 3 materias.", problem.Detail);
    }

    [Fact]
    public async Task CreateStudent_ShouldReject_WhenLessThanThreeSubjectsAreSelected()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var invalidSelection = SelectSubjectsFromDistinctProfessors(subjects, 2)
            .Select(subject => subject.Id)
            .ToArray();

        var response = await client.PostAsJsonAsync("/api/students", new StudentRequest(
            "Diana Prince",
            "diana@example.com",
            "Software Engineering",
            "Test1234!",
            invalidSelection));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await ReadProblemAsync(response);
        Assert.Equal(400, problem.Status);
        Assert.Equal("Un estudiante debe seleccionar exactamente 3 materias.", problem.Detail);
    }

    [Fact]
    public async Task StudentDetail_ShouldIncludeClassmatesGroupedBySubject()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var distinctSubjects = SelectSubjectsFromDistinctProfessors(subjects, 5);

        var sharedSubject = distinctSubjects[0];
        var secondAliceSubject = distinctSubjects[1];
        var thirdAliceSubject = distinctSubjects[2];
        var secondBobSubject = distinctSubjects[3];
        var thirdBobSubject = distinctSubjects[4];

        var alice = await CreateStudentAsync(client, new StudentRequest(
            "Alice Walker",
            "alice.walker@example.com",
            "Software Engineering",
            "Test1234!",
            [sharedSubject.Id, secondAliceSubject.Id, thirdAliceSubject.Id]));

        await CreateStudentAsync(client, new StudentRequest(
            "Bruno Diaz",
            "bruno.diaz@example.com",
            "Software Engineering",
            "Test1234!",
            [sharedSubject.Id, secondBobSubject.Id, thirdBobSubject.Id]));

        var detail = await client.GetFromJsonAsync<StudentDetailResponse>($"/api/students/{alice.Id}");

        Assert.NotNull(detail);
        Assert.NotNull(detail.Enrollment);

        var sharedSubjectDetail = detail.Enrollment.Subjects.Single(subject => subject.SubjectId == sharedSubject.Id);
        var privateSubjectDetail = detail.Enrollment.Subjects.Single(subject => subject.SubjectId == secondAliceSubject.Id);

        Assert.Contains("Bruno Diaz", sharedSubjectDetail.Classmates);
        Assert.DoesNotContain("Alice Walker", sharedSubjectDetail.Classmates);
        Assert.Empty(privateSubjectDetail.Classmates);
    }

    [Fact]
    public async Task StudentRole_ShouldOnlyAccessOwnDetail()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var studentSelection = SelectSubjectsFromDistinctProfessors(subjects, 3);

        var alice = await CreateStudentAsync(client, new StudentRequest(
            "Alice Access",
            "alice.access@example.com",
            "Software Engineering",
            "Test1234!",
            studentSelection.Select(subject => subject.Id).ToArray()));

        var bob = await CreateStudentAsync(client, new StudentRequest(
            "Bob Access",
            "bob.access@example.com",
            "Computer Science",
            "Test1234!",
            SelectSubjectsFromDistinctProfessors(subjects.Skip(1).ToList(), 3).Select(subject => subject.Id).ToArray()));

        await AuthenticateAsStudentAsync(client, "alice.access@example.com", "Test1234!");

        var ownResponse = await client.GetAsync($"/api/students/{alice.Id}");
        var otherResponse = await client.GetAsync($"/api/students/{bob.Id}");

        Assert.Equal(HttpStatusCode.OK, ownResponse.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, otherResponse.StatusCode);
    }

    [Fact]
    public async Task StudentRole_ShouldNotCreateAnotherStudent()
    {
        using var factory = new StudentsPlatformApiFactory();
        using var client = factory.CreateClient();
        await AuthenticateAsAdminAsync(client);

        var subjects = await GetSubjectsAsync(client);
        var studentSelection = SelectSubjectsFromDistinctProfessors(subjects, 3);

        await CreateStudentAsync(client, new StudentRequest(
            "Student User",
            "student.user@example.com",
            "Software Engineering",
            "Test1234!",
            studentSelection.Select(subject => subject.Id).ToArray()));

        await AuthenticateAsStudentAsync(client, "student.user@example.com", "Test1234!");

        var response = await client.PostAsJsonAsync("/api/students", new StudentRequest(
            "Forbidden Creation",
            "forbidden.creation@example.com",
            "Industrial Engineering",
            "Test1234!",
            studentSelection.Select(subject => subject.Id).ToArray()));

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    private static async Task AuthenticateAsAdminAsync(HttpClient client)
    {
        await AuthenticateAsync(client, new LoginRequest("admin@students.local", "Admin1234!"));
    }

    private static async Task AuthenticateAsStudentAsync(HttpClient client, string email, string password)
    {
        await AuthenticateAsync(client, new LoginRequest(email, password));
    }

    private static async Task AuthenticateAsync(HttpClient client, LoginRequest request)
    {
        client.DefaultRequestHeaders.Authorization = null;

        var response = await client.PostAsJsonAsync("/api/auth/login", request);
        response.EnsureSuccessStatusCode();

        var session = await response.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session!.AccessToken);
    }

    private static async Task<List<SubjectResponse>> GetSubjectsAsync(HttpClient client)
    {
        var subjects = await client.GetFromJsonAsync<List<SubjectResponse>>("/api/subjects");
        return subjects ?? throw new InvalidOperationException("Subjects catalog was not returned.");
    }

    private static async Task<StudentDetailResponse> CreateStudentAsync(HttpClient client, StudentRequest request)
    {
        var response = await client.PostAsJsonAsync("/api/students", request);
        response.EnsureSuccessStatusCode();

        var student = await response.Content.ReadFromJsonAsync<StudentDetailResponse>();
        return student ?? throw new InvalidOperationException("Student detail was not returned.");
    }

    private static List<SubjectResponse> SelectSubjectsFromDistinctProfessors(
        IReadOnlyCollection<SubjectResponse> subjects,
        int count)
    {
        return subjects
            .GroupBy(subject => subject.ProfessorId)
            .Take(count)
            .Select(group => group.First())
            .ToList();
    }

    private static async Task<ProblemResponse> ReadProblemAsync(HttpResponseMessage response)
    {
        using var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = jsonDocument.RootElement;

        Dictionary<string, string[]>? errors = null;
        if (root.TryGetProperty("errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Object)
        {
            errors = errorsElement.EnumerateObject()
                .ToDictionary(
                    property => property.Name,
                    property => property.Value.EnumerateArray().Select(item => item.GetString() ?? string.Empty).ToArray());
        }

        return new ProblemResponse(
            root.TryGetProperty("title", out var title) ? title.GetString() : null,
            root.TryGetProperty("status", out var status) ? status.GetInt32() : null,
            root.TryGetProperty("detail", out var detail) ? detail.GetString() : null,
            errors);
    }

    private sealed record LoginRequest(string Email, string Password);

    private sealed record LoginResponse(string AccessToken);

    private sealed record StudentRequest(
        string FullName,
        string Email,
        string ProgramName,
        string? Password,
        IReadOnlyCollection<Guid> SubjectIds);

    private sealed record SubjectsRequest(IReadOnlyCollection<Guid> SubjectIds);

    private sealed record SubjectResponse(Guid Id, string Name, int Credits, Guid ProfessorId, string ProfessorName);

    private sealed record StudentListItemResponse(
        Guid Id,
        string FullName,
        string Email,
        string ProgramName,
        int SubjectsCount,
        int Credits,
        DateTimeOffset CreatedAt);

    private sealed record StudentDetailResponse(
        Guid Id,
        string FullName,
        string Email,
        string ProgramName,
        DateTimeOffset CreatedAt,
        EnrollmentResponse? Enrollment);

    private sealed record EnrollmentResponse(
        Guid Id,
        DateTimeOffset CreatedAt,
        int TotalCredits,
        List<EnrolledSubjectResponse> Subjects);

    private sealed record EnrolledSubjectResponse(
        Guid SubjectId,
        string SubjectName,
        int Credits,
        Guid ProfessorId,
        string ProfessorName,
        List<string> Classmates);

    private sealed record ProblemResponse(
        string? Title,
        int? Status,
        string? Detail,
        Dictionary<string, string[]>? Errors);
}
