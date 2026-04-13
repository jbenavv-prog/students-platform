using Microsoft.Extensions.Options;
using StudentsPlatform.Application.Common.Abstractions;

namespace StudentsPlatform.Api.Configuration;

public sealed class ConfiguredAdminAccountProvider(IOptions<AuthOptions> options) : IAdminAccountProvider
{
    public AdminAccount? FindByEmail(string normalizedEmail)
    {
        var admin = options.Value.Admin;
        if (string.IsNullOrWhiteSpace(admin.Email) || string.IsNullOrWhiteSpace(admin.Password))
        {
            return null;
        }

        return string.Equals(admin.Email.Trim(), normalizedEmail, StringComparison.OrdinalIgnoreCase)
            ? new AdminAccount(admin.Id, admin.FullName.Trim(), admin.Email.Trim(), admin.Password)
            : null;
    }
}
