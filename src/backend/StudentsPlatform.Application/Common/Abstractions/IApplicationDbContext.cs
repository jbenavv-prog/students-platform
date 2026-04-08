using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Domain.Catalog;
using StudentsPlatform.Domain.Enrollments;
using StudentsPlatform.Domain.Students;

namespace StudentsPlatform.Application.Common.Abstractions;

public interface IApplicationDbContext
{
    DbSet<Student> Students { get; }

    DbSet<Professor> Professors { get; }

    DbSet<Subject> Subjects { get; }

    DbSet<Enrollment> Enrollments { get; }

    DbSet<EnrollmentSubject> EnrollmentSubjects { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
