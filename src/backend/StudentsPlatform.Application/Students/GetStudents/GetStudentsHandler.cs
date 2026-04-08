using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Application.Students.Shared;

namespace StudentsPlatform.Application.Students.GetStudents;

public sealed class GetStudentsHandler(IApplicationDbContext dbContext)
{
    public async Task<IReadOnlyCollection<StudentListItemDto>> HandleAsync(CancellationToken cancellationToken)
    {
        var students = await dbContext.Students
            .AsNoTracking()
            .Include(student => student.Enrollment)
            .ThenInclude(enrollment => enrollment!.Subjects)
            .OrderBy(student => student.FullName)
            .ToListAsync(cancellationToken);

        var subjectIds = students
            .Where(student => student.Enrollment is not null)
            .SelectMany(student => student.Enrollment!.Subjects.Select(subject => subject.SubjectId))
            .Distinct()
            .ToArray();

        var creditsBySubjectId = subjectIds.Length == 0
            ? new Dictionary<Guid, int>()
            : await dbContext.Subjects
                .AsNoTracking()
                .Where(subject => subjectIds.Contains(subject.Id))
                .ToDictionaryAsync(subject => subject.Id, subject => subject.Credits, cancellationToken);

        return students
            .Select(student =>
            {
                var selectedSubjectIds = student.Enrollment?.Subjects.Select(subject => subject.SubjectId).ToArray() ?? Array.Empty<Guid>();

                return new StudentListItemDto(
                    student.Id,
                    student.FullName,
                    student.Email,
                    student.ProgramName,
                    selectedSubjectIds.Length,
                    selectedSubjectIds.Sum(subjectId => creditsBySubjectId.GetValueOrDefault(subjectId)),
                    student.CreatedAt);
            })
            .ToArray();
    }
}
