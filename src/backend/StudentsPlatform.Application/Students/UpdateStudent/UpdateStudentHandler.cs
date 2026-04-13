using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Exceptions;
using StudentsPlatform.Application.Students.Shared;
using StudentsPlatform.Domain.Enrollments;

namespace StudentsPlatform.Application.Students.UpdateStudent;

public sealed class UpdateStudentHandler(
    IApplicationDbContext dbContext,
    IAdminAccountProvider adminAccountProvider,
    IPasswordHashingService passwordHashingService,
    ILogger<UpdateStudentHandler> logger)
{
    public async Task<StudentDetailDto> HandleAsync(UpdateStudentCommand command, CancellationToken cancellationToken)
    {
        StudentRequestValidator.ValidateProfile(command.FullName, command.Email, command.ProgramName);
        StudentRequestValidator.ValidatePasswordOptional(command.Password);
        var selectedSubjects = await StudentDataAccess.LoadSelectedSubjectsAsync(dbContext, command.SubjectIds, cancellationToken);

        var student = await StudentDataAccess.GetStudentForUpdateAsync(dbContext, command.StudentId, cancellationToken);

        var emailExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(entity => entity.Id != command.StudentId && entity.Email == command.Email.Trim(), cancellationToken);

        if (emailExists || adminAccountProvider.FindByEmail(command.Email.Trim()) is not null)
        {
            throw new ConflictException("Ya existe un estudiante registrado con ese correo.");
        }

        student.UpdateProfile(command.FullName, command.Email, command.ProgramName);

        if (!string.IsNullOrWhiteSpace(command.Password))
        {
            var passwordHash = passwordHashingService.HashPassword(command.Password);
            student.SetPasswordHash(passwordHash);
        }

        if (student.Enrollment is null)
        {
            student.SetEnrollment(Enrollment.Create(student.Id, DateTimeOffset.UtcNow));
        }

        student.Enrollment!.ReplaceSubjects(selectedSubjects);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Student {StudentId} updated with {SubjectsCount} selected subjects.", student.Id, selectedSubjects.Count);

        return await StudentDataAccess.BuildStudentDetailAsync(dbContext, student.Id, cancellationToken);
    }
}
