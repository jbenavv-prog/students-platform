using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;

namespace StudentsPlatform.Application.Catalog.Professors;

public sealed class GetProfessorsHandler(IApplicationDbContext dbContext)
{
    public async Task<IReadOnlyCollection<ProfessorDto>> HandleAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Professors
            .AsNoTracking()
            .OrderBy(professor => professor.FullName)
            .Select(professor => new ProfessorDto(professor.Id, professor.FullName))
            .ToListAsync(cancellationToken);
    }
}
