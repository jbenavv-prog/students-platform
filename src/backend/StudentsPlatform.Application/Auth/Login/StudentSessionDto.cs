namespace StudentsPlatform.Application.Auth.Login;

public sealed record StudentSessionDto(
    Guid Id,
    string FullName,
    string Email,
    string? ProgramName,
    string Role,
    string AccessToken);
