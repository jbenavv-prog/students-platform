namespace StudentsPlatform.Api.Modules.Students.Requests;

public sealed record UpdateStudentSubjectsRequest(IReadOnlyCollection<Guid>? SubjectIds);
