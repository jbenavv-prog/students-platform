using StudentsPlatform.Domain.Common;
using StudentsPlatform.Domain.Enrollments;

namespace StudentsPlatform.Domain.Students;

public sealed class Student
{
    private Student()
    {
    }

    private Student(Guid id, string fullName, string email, string programName, DateTimeOffset createdAt)
    {
        Id = id;
        FullName = Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Email = Guard.AgainstInvalidEmail(email, nameof(email));
        ProgramName = Guard.AgainstNullOrWhiteSpace(programName, nameof(programName));
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string ProgramName { get; private set; } = string.Empty;

    public string? PasswordHash { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public Enrollment? Enrollment { get; private set; }

    public static Student Create(string fullName, string email, string programName, DateTimeOffset createdAt)
    {
        return new Student(Guid.NewGuid(), fullName, email, programName, createdAt);
    }

    public void UpdateProfile(string fullName, string email, string programName)
    {
        FullName = Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
        Email = Guard.AgainstInvalidEmail(email, nameof(email));
        ProgramName = Guard.AgainstNullOrWhiteSpace(programName, nameof(programName));
    }

    public void SetPasswordHash(string passwordHash)
    {
        PasswordHash = Guard.AgainstNullOrWhiteSpace(passwordHash, nameof(passwordHash));
    }

    public void SetEnrollment(Enrollment enrollment)
    {
        Enrollment = enrollment ?? throw new ArgumentNullException(nameof(enrollment));
    }

    public void ClearEnrollment()
    {
        Enrollment = null;
    }
}
