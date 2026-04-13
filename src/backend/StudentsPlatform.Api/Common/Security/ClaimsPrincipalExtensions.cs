using System.Security.Claims;
using StudentsPlatform.Application.Common.Security;

namespace StudentsPlatform.Api.Common.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal principal)
    {
        var rawValue = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(rawValue, out var userId)
            ? userId
            : null;
    }

    public static bool IsAdministrator(this ClaimsPrincipal principal)
    {
        return principal.IsInRole(AppRoles.Administrator);
    }

    public static bool CanAccessStudent(this ClaimsPrincipal principal, Guid studentId)
    {
        return principal.IsAdministrator() || principal.GetUserId() == studentId;
    }
}
