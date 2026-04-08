namespace StudentsPlatform.Application.Students.Shared;

public sealed record StudentDetailDto(
    Guid Id,
    string FullName,
    string Email,
    string ProgramName,
    DateTimeOffset CreatedAt,
    EnrollmentDetailDto? Enrollment);

public sealed record EnrollmentDetailDto(
    Guid Id,
    DateTimeOffset CreatedAt,
    int TotalCredits,
    IReadOnlyCollection<EnrolledSubjectDetailDto> Subjects);

public sealed record EnrolledSubjectDetailDto(
    Guid SubjectId,
    string SubjectName,
    int Credits,
    Guid ProfessorId,
    string ProfessorName,
    IReadOnlyCollection<string> Classmates);
