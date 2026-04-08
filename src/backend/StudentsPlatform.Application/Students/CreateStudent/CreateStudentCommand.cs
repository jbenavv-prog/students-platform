namespace StudentsPlatform.Application.Students.CreateStudent;

public sealed record CreateStudentCommand(
    string FullName,
    string Email,
    string ProgramName,
    IReadOnlyCollection<Guid> SubjectIds);
