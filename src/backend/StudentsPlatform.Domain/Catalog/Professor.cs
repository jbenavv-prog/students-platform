using StudentsPlatform.Domain.Common;

namespace StudentsPlatform.Domain.Catalog;

public sealed class Professor
{
    private Professor()
    {
    }

    private Professor(Guid id, string fullName)
    {
        Id = id;
        FullName = Guard.AgainstNullOrWhiteSpace(fullName, nameof(fullName));
    }

    public Guid Id { get; private set; }

    public string FullName { get; private set; } = string.Empty;

    public static Professor Create(Guid id, string fullName)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Professor id is required.", nameof(id));
        }

        return new Professor(id, fullName);
    }
}
