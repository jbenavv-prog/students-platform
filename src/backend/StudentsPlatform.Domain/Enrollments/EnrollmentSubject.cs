namespace StudentsPlatform.Domain.Enrollments;

public sealed class EnrollmentSubject
{
    private EnrollmentSubject()
    {
    }

    private EnrollmentSubject(Guid enrollmentId, Guid subjectId)
    {
        EnrollmentId = enrollmentId;
        SubjectId = subjectId;
    }

    public Guid EnrollmentId { get; private set; }

    public Guid SubjectId { get; private set; }

    public static EnrollmentSubject Create(Guid enrollmentId, Guid subjectId)
    {
        if (enrollmentId == Guid.Empty)
        {
            throw new ArgumentException("Enrollment id is required.", nameof(enrollmentId));
        }

        if (subjectId == Guid.Empty)
        {
            throw new ArgumentException("Subject id is required.", nameof(subjectId));
        }

        return new EnrollmentSubject(enrollmentId, subjectId);
    }
}
