namespace StudentsPlatform.Application.Catalog.Subjects;

public sealed record SubjectDto(
    Guid Id,
    string Name,
    int Credits,
    Guid ProfessorId,
    string ProfessorName);
