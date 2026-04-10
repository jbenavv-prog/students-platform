namespace StudentsPlatform.Api.Modules.Students.Requests;

public sealed record UpsertStudentRequest(
    string FullName,
    string Email,
    string ProgramName,
    string? Password,
    IReadOnlyCollection<Guid>? SubjectIds);
