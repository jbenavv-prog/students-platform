using Microsoft.EntityFrameworkCore;
using StudentsPlatform.Application.Common.Abstractions;
using StudentsPlatform.Domain.Catalog;
using StudentsPlatform.Domain.Enrollments;
using StudentsPlatform.Domain.Students;

namespace StudentsPlatform.Infrastructure.Persistence;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options), IApplicationDbContext
{
    public DbSet<Student> Students => Set<Student>();

    public DbSet<Professor> Professors => Set<Professor>();

    public DbSet<Subject> Subjects => Set<Subject>();

    public DbSet<Enrollment> Enrollments => Set<Enrollment>();

    public DbSet<EnrollmentSubject> EnrollmentSubjects => Set<EnrollmentSubject>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Student>(builder =>
        {
            builder.ToTable("students");
            builder.HasKey(student => student.Id);
            builder.Property(student => student.FullName).HasMaxLength(150).IsRequired();
            builder.Property(student => student.Email).HasMaxLength(200).IsRequired();
            builder.Property(student => student.ProgramName).HasMaxLength(150).IsRequired();
            builder.Property(student => student.CreatedAt).IsRequired();
            builder.HasIndex(student => student.Email).IsUnique();

            builder.HasOne(student => student.Enrollment)
                .WithOne()
                .HasForeignKey<Enrollment>(enrollment => enrollment.StudentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Professor>(builder =>
        {
            builder.ToTable("professors");
            builder.HasKey(professor => professor.Id);
            builder.Property(professor => professor.FullName).HasMaxLength(150).IsRequired();
        });

        modelBuilder.Entity<Subject>(builder =>
        {
            builder.ToTable("subjects");
            builder.HasKey(subject => subject.Id);
            builder.Property(subject => subject.Name).HasMaxLength(150).IsRequired();
            builder.Property(subject => subject.Credits).IsRequired();
            builder.Property(subject => subject.ProfessorId).IsRequired();

            builder.HasOne(subject => subject.Professor)
                .WithMany()
                .HasForeignKey(subject => subject.ProfessorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Enrollment>(builder =>
        {
            builder.ToTable("enrollments");
            builder.HasKey(enrollment => enrollment.Id);
            builder.Property(enrollment => enrollment.StudentId).IsRequired();
            builder.Property(enrollment => enrollment.CreatedAt).IsRequired();

            builder.Navigation(enrollment => enrollment.Subjects)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(enrollment => enrollment.Subjects)
                .WithOne()
                .HasForeignKey(subject => subject.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<EnrollmentSubject>(builder =>
        {
            builder.ToTable("enrollment_subjects");
            builder.HasKey(subject => new { subject.EnrollmentId, subject.SubjectId });
            builder.Property(subject => subject.EnrollmentId).IsRequired();
            builder.Property(subject => subject.SubjectId).IsRequired();

            builder.HasOne<Subject>()
                .WithMany()
                .HasForeignKey(subject => subject.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
