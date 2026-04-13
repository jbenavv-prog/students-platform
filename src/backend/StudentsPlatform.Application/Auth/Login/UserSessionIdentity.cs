namespace StudentsPlatform.Application.Auth.Login;

public sealed record UserSessionIdentity(
    Guid Id,
    string FullName,
    string Email,
    string Role);
