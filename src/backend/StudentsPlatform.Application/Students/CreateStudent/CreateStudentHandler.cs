using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Exceptions;
using StudentsPlatform.Application.Students.Shared;
using StudentsPlatform.Domain.Enrollments;
using StudentsPlatform.Domain.Students;

namespace StudentsPlatform.Application.Students.CreateStudent;

public sealed class CreateStudentHandler(
    IApplicationDbContext dbContext,
    IPasswordHashingService passwordHashingService,
    ILogger<CreateStudentHandler> logger)
{
    public async Task<StudentDetailDto> HandleAsync(CreateStudentCommand command, CancellationToken cancellationToken)
    {
        StudentRequestValidator.ValidateProfile(command.FullName, command.Email, command.ProgramName);
        StudentRequestValidator.ValidatePasswordRequired(command.Password);
        var selectedSubjects = await StudentDataAccess.LoadSelectedSubjectsAsync(dbContext, command.SubjectIds, cancellationToken);

        var emailExists = await dbContext.Students
            .AsNoTracking()
            .AnyAsync(student => student.Email == command.Email.Trim(), cancellationToken);

        if (emailExists)
        {
            throw new ConflictException("Ya existe un estudiante registrado con ese correo.");
        }

        var student = Student.Create(
            command.FullName,
            command.Email,
            command.ProgramName,
            DateTimeOffset.UtcNow);

        var passwordHash = passwordHashingService.HashPassword(command.Password);
        student.SetPasswordHash(passwordHash);

        var enrollment = Enrollment.Create(student.Id, DateTimeOffset.UtcNow);
        enrollment.ReplaceSubjects(selectedSubjects);
        student.SetEnrollment(enrollment);

        await dbContext.Students.AddAsync(student, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Student {StudentId} created with {SubjectsCount} selected subjects.", student.Id, selectedSubjects.Count);

        return await StudentDataAccess.BuildStudentDetailAsync(dbContext, student.Id, cancellationToken);
    }
}
