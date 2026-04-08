namespace StudentsPlatform.Application.Students.Shared;

public sealed record StudentListItemDto(
    Guid Id,
    string FullName,
    string Email,
    string ProgramName,
    int SubjectsCount,
    int Credits,
    DateTimeOffset CreatedAt);
