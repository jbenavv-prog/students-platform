using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Common.Exceptions;

namespace StudentsPlatform.Application.Students.DeleteStudent;

public sealed class DeleteStudentHandler(
    IApplicationDbContext dbContext,
    ILogger<DeleteStudentHandler> logger)
{
    public async Task HandleAsync(Guid studentId, CancellationToken cancellationToken)
    {
        var student = await dbContext.Students
            .Include(entity => entity.Enrollment)
            .ThenInclude(enrollment => enrollment!.Subjects)
            .SingleOrDefaultAsync(entity => entity.Id == studentId, cancellationToken);

        if (student is null)
        {
            throw new NotFoundException("Student", studentId);
        }

        dbContext.Students.Remove(student);
        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Student {StudentId} deleted.", studentId);
    }
}
