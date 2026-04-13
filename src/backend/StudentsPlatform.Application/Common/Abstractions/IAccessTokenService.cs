using System.Security.Claims;
using StudentsPlatform.Application.Auth.Login;

namespace StudentsPlatform.Application.Common.Abstractions;

public interface IAccessTokenService
{
    string CreateToken(UserSessionIdentity identity);

    ClaimsPrincipal? ValidateToken(string token);
}
