namespace StudentsPlatform.Application.Students.UpdateStudentSubjects;

public sealed record UpdateStudentSubjectsCommand(
    Guid StudentId,
    IReadOnlyCollection<Guid> SubjectIds);
