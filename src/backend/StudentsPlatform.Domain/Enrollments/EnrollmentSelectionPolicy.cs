using StudentsPlatform.Domain.Catalog;
using StudentsPlatform.Domain.Common;

namespace StudentsPlatform.Domain.Enrollments;

public static class EnrollmentSelectionPolicy
{
    public const int MaxSubjects = 3;
    public const int MaxCredits = 9;

    public static void Validate(IReadOnlyCollection<Subject> subjects)
    {
        ArgumentNullException.ThrowIfNull(subjects);

        if (subjects.Count != MaxSubjects)
        {
            throw new DomainRuleException("Un estudiante debe seleccionar exactamente 3 materias.");
        }

        if (subjects.GroupBy(subject => subject.Id).Any(group => group.Count() > 1))
        {
            throw new DomainRuleException("No se puede repetir una materia dentro de la misma inscripcion.");
        }

        if (subjects.GroupBy(subject => subject.ProfessorId).Any(group => group.Count() > 1))
        {
            throw new DomainRuleException("No puedes seleccionar dos materias del mismo profesor.");
        }

        var totalCredits = subjects.Sum(subject => subject.Credits);
        if (totalCredits > MaxCredits)
        {
            throw new DomainRuleException("Una inscripcion no puede superar 9 creditos.");
        }
    }
}
