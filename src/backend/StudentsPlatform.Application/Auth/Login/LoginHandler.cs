using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Exceptions;
using StudentsPlatform.Application.Common.Security;

namespace StudentsPlatform.Application.Auth.Login;

public sealed class LoginHandler(
    IApplicationDbContext dbContext,
    IPasswordHashingService passwordHashingService,
    IAdminAccountProvider adminAccountProvider,
    IAccessTokenService accessTokenService)
{
    public async Task<StudentSessionDto> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        LoginRequestValidator.Validate(command.Email, command.Password);

        var normalizedEmail = command.Email.Trim();
        var adminAccount = adminAccountProvider.FindByEmail(normalizedEmail);

        if (adminAccount is not null)
        {
            if (!string.Equals(command.Password, adminAccount.Password, StringComparison.Ordinal))
            {
                throw new UnauthorizedException("Credenciales invalidas.");
            }

            var adminIdentity = new UserSessionIdentity(
                adminAccount.Id,
                adminAccount.FullName,
                adminAccount.Email,
                AppRoles.Administrator);

            return new StudentSessionDto(
                adminAccount.Id,
                adminAccount.FullName,
                adminAccount.Email,
                null,
                AppRoles.Administrator,
                accessTokenService.CreateToken(adminIdentity));
        }

        var student = await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Email == normalizedEmail, cancellationToken);

        if (student is null || !passwordHashingService.VerifyPassword(command.Password, student.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        var studentIdentity = new UserSessionIdentity(
            student.Id,
            student.FullName,
            student.Email,
            AppRoles.Student);

        return new StudentSessionDto(
            student.Id,
            student.FullName,
            student.Email,
            student.ProgramName,
            AppRoles.Student,
            accessTokenService.CreateToken(studentIdentity));
    }
}
