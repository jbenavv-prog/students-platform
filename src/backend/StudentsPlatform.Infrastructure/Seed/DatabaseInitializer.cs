using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StudentsPlatform.Infrastructure.Persistence;

namespace StudentsPlatform.Infrastructure.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(ApplicationDbContext dbContext, ILogger logger, CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (!await dbContext.Professors.AnyAsync(cancellationToken))
        {
            await dbContext.Professors.AddRangeAsync(CatalogSeedData.Professors, cancellationToken);
        }

        if (!await dbContext.Subjects.AnyAsync(cancellationToken))
        {
            await dbContext.Subjects.AddRangeAsync(CatalogSeedData.Subjects, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        logger.LogInformation(
            "Database initialized with {ProfessorsCount} professors and {SubjectsCount} subjects.",
            CatalogSeedData.Professors.Count,
            CatalogSeedData.Subjects.Count);
    }
}
