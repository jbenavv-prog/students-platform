using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Exceptions;

namespace StudentsPlatform.Application.Auth.Login;

public sealed class LoginHandler(
    IApplicationDbContext dbContext,
    IPasswordHashingService passwordHashingService)
{
    public async Task<StudentSessionDto> HandleAsync(LoginCommand command, CancellationToken cancellationToken)
    {
        LoginRequestValidator.Validate(command.Email, command.Password);

        var normalizedEmail = command.Email.Trim();

        var student = await dbContext.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(entity => entity.Email == normalizedEmail, cancellationToken);

        if (student is null || !passwordHashingService.VerifyPassword(command.Password, student.PasswordHash))
        {
            throw new UnauthorizedException("Credenciales invalidas.");
        }

        return new StudentSessionDto(
            student.Id,
            student.FullName,
            student.Email,
            student.ProgramName);
    }
}
