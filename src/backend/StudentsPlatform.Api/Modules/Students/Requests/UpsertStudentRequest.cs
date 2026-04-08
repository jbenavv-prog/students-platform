namespace StudentsPlatform.Api.Modules.Students.Requests;

public sealed record UpsertStudentRequest(
    string FullName,
    string Email,
    string ProgramName,
    IReadOnlyCollection<Guid>? SubjectIds);
