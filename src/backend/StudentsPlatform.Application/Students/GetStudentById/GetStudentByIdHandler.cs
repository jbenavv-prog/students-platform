using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Students.Shared;

namespace StudentsPlatform.Application.Students.GetStudentById;

public sealed class GetStudentByIdHandler(IApplicationDbContext dbContext)
{
    public Task<StudentDetailDto> HandleAsync(Guid studentId, CancellationToken cancellationToken)
    {
        return StudentDataAccess.BuildStudentDetailAsync(dbContext, studentId, cancellationToken);
    }
}
