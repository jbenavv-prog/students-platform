using Microsoft.Extensions.Logging;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Students.Shared;
using StudentsPlatform.Domain.Enrollments;

namespace StudentsPlatform.Application.Students.UpdateStudentSubjects;

public sealed class UpdateStudentSubjectsHandler(
    IApplicationDbContext dbContext,
    ILogger<UpdateStudentSubjectsHandler> logger)
{
    public async Task<StudentDetailDto> HandleAsync(UpdateStudentSubjectsCommand command, CancellationToken cancellationToken)
    {
        var selectedSubjects = await StudentDataAccess.LoadSelectedSubjectsAsync(dbContext, command.SubjectIds, cancellationToken);
        var student = await StudentDataAccess.GetStudentForUpdateAsync(dbContext, command.StudentId, cancellationToken);

        if (student.Enrollment is null)
        {
            student.SetEnrollment(Enrollment.Create(student.Id, DateTimeOffset.UtcNow));
        }

        student.Enrollment!.ReplaceSubjects(selectedSubjects);

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Student {StudentId} subjects updated. Selection count: {SubjectsCount}.", student.Id, selectedSubjects.Count);

        return await StudentDataAccess.BuildStudentDetailAsync(dbContext, student.Id, cancellationToken);
    }
}
