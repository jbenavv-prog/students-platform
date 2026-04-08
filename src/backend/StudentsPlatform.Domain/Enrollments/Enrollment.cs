using StudentsPlatform.Domain.Catalog;

namespace StudentsPlatform.Domain.Enrollments;

public sealed class Enrollment
{
    private readonly List<EnrollmentSubject> _subjects = [];

    private Enrollment()
    {
    }

    private Enrollment(Guid id, Guid studentId, DateTimeOffset createdAt)
    {
        Id = id;
        StudentId = studentId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; private set; }

    public Guid StudentId { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public IReadOnlyCollection<EnrollmentSubject> Subjects => _subjects.AsReadOnly();

    public static Enrollment Create(Guid studentId, DateTimeOffset createdAt)
    {
        if (studentId == Guid.Empty)
        {
            throw new ArgumentException("Student id is required.", nameof(studentId));
        }

        return new Enrollment(Guid.NewGuid(), studentId, createdAt);
    }

    public void ReplaceSubjects(IReadOnlyCollection<Subject> subjects)
    {
        ArgumentNullException.ThrowIfNull(subjects);

        EnrollmentSelectionPolicy.Validate(subjects);

        _subjects.Clear();

        foreach (var subject in subjects)
        {
            _subjects.Add(EnrollmentSubject.Create(Id, subject.Id));
        }
    }
}
