namespace StudentsPlatform.Application.Common.Abstractions;

public interface IAdminAccountProvider
{
    AdminAccount? FindByEmail(string normalizedEmail);
}

public sealed record AdminAccount(
    Guid Id,
    string FullName,
    string Email,
    string Password);
