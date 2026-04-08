using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;

namespace StudentsPlatform.Application.Catalog.Subjects;

public sealed class GetSubjectsHandler(IApplicationDbContext dbContext)
{
    public async Task<IReadOnlyCollection<SubjectDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Subjects
            .AsNoTracking()
            .Include(subject => subject.Professor)
            .OrderBy(subject => subject.Name)
            .Select(subject => new SubjectDto(
                subject.Id,
                subject.Name,
                subject.Credits,
                subject.ProfessorId,
                subject.Professor!.FullName))
            .ToListAsync(cancellationToken);
    }
}
