namespace StudentsPlatform.Application.Students.CreateStudent;

public sealed record CreateStudentCommand(
    string FullName,
    string Email,
    string ProgramName,
    string Password,
    IReadOnlyCollection<Guid> SubjectIds);
