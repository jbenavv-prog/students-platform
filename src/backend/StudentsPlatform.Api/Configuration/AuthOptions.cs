namespace StudentsPlatform.Api.Configuration;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string SigningKey { get; init; } = string.Empty;

    public int TokenExpirationMinutes { get; init; } = 480;

    public AdminAccountOptions Admin { get; init; } = new();
}

public sealed class AdminAccountOptions
{
    public Guid Id { get; init; } = Guid.Parse("1c9f2d84-76fd-4a0c-93c5-bc981a7d1496");

    public string FullName { get; init; } = "Administrador Students Platform";

    public string Email { get; init; } = "admin@students.local";

    public string Password { get; init; } = "Admin1234!";
}
