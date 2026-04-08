using StudentsPlatform.Domain.Common;

namespace StudentsPlatform.Domain.Catalog;

public sealed class Subject
{
    private Subject()
    {
    }

    private Subject(Guid id, string name, int credits, Guid professorId)
    {
        Id = id;
        Name = Guard.AgainstNullOrWhiteSpace(name, nameof(name));
        Credits = credits > 0 ? credits : throw new ArgumentOutOfRangeException(nameof(credits));
        ProfessorId = professorId != Guid.Empty
            ? professorId
            : throw new ArgumentException("Professor id is required.", nameof(professorId));
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public int Credits { get; private set; }

    public Guid ProfessorId { get; private set; }

    public Professor? Professor { get; private set; }

    public static Subject Create(Guid id, string name, int credits, Guid professorId)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Subject id is required.", nameof(id));
        }

        return new Subject(id, name, credits, professorId);
    }
}
