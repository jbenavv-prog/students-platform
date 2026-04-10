namespace StudentsPlatform.Application.Students.UpdateStudent;

public sealed record UpdateStudentCommand(
    Guid StudentId,
    string FullName,
    string Email,
    string ProgramName,
    string? Password,
    IReadOnlyCollection<Guid> SubjectIds);
