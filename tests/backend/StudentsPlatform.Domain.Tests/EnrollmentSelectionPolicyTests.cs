using StudentsPlatform.Domain.Catalog;
using StudentsPlatform.Domain.Common;
using StudentsPlatform.Domain.Enrollments;

namespace StudentsPlatform.Domain.Tests;

public sealed class EnrollmentSelectionPolicyTests
{
    [Fact]
    public void Validate_ShouldReject_WhenMoreThanThreeSubjectsAreSelected()
    {
        var subjects = new[]
        {
            CreateSubject("Calculo I", Guid.NewGuid()),
            CreateSubject("Programacion I", Guid.NewGuid()),
            CreateSubject("Bases de Datos", Guid.NewGuid()),
            CreateSubject("Redes", Guid.NewGuid())
        };

        var exception = Assert.Throws<DomainRuleException>(() => EnrollmentSelectionPolicy.Validate(subjects));

        Assert.Equal("Un estudiante debe seleccionar exactamente 3 materias.", exception.Message);
    }

    [Fact]
    public void Validate_ShouldReject_WhenLessThanThreeSubjectsAreSelected()
    {
        var subjects = new[]
        {
            CreateSubject("Calculo I", Guid.NewGuid()),
            CreateSubject("Programacion I", Guid.NewGuid())
        };

        var exception = Assert.Throws<DomainRuleException>(() => EnrollmentSelectionPolicy.Validate(subjects));

        Assert.Equal("Un estudiante debe seleccionar exactamente 3 materias.", exception.Message);
    }

    [Fact]
    public void Validate_ShouldReject_WhenTheSameSubjectIsRepeated()
    {
        var subjectId = Guid.NewGuid();
        var professorOne = Guid.NewGuid();
        var professorTwo = Guid.NewGuid();

        var subjects = new[]
        {
            Subject.Create(subjectId, "Programacion I", 3, professorOne),
            Subject.Create(subjectId, "Programacion I", 3, professorOne),
            CreateSubject("Bases de Datos", professorTwo)
        };

        var exception = Assert.Throws<DomainRuleException>(() => EnrollmentSelectionPolicy.Validate(subjects));

        Assert.Equal("No se puede repetir una materia dentro de la misma inscripcion.", exception.Message);
    }

    [Fact]
    public void Validate_ShouldReject_WhenTwoSubjectsHaveTheSameProfessor()
    {
        var professorId = Guid.NewGuid();

        var subjects = new[]
        {
            CreateSubject("Calculo I", professorId),
            CreateSubject("Algebra Lineal", professorId),
            CreateSubject("Bases de Datos", Guid.NewGuid())
        };

        var exception = Assert.Throws<DomainRuleException>(() => EnrollmentSelectionPolicy.Validate(subjects));

        Assert.Equal("No puedes seleccionar dos materias del mismo profesor.", exception.Message);
    }

    [Fact]
    public void Validate_ShouldReject_WhenSelectedCreditsExceedNine()
    {
        var subjects = new[]
        {
            Subject.Create(Guid.NewGuid(), "Calculo I", 4, Guid.NewGuid()),
            Subject.Create(Guid.NewGuid(), "Programacion I", 4, Guid.NewGuid()),
            Subject.Create(Guid.NewGuid(), "Bases de Datos", 4, Guid.NewGuid())
        };

        var exception = Assert.Throws<DomainRuleException>(() => EnrollmentSelectionPolicy.Validate(subjects));

        Assert.Equal("Una inscripcion no puede superar 9 creditos.", exception.Message);
    }

    [Fact]
    public void Validate_ShouldAccept_WhenSelectionIsValid()
    {
        var subjects = new[]
        {
            CreateSubject("Calculo I", Guid.NewGuid()),
            CreateSubject("Programacion I", Guid.NewGuid()),
            CreateSubject("Bases de Datos", Guid.NewGuid())
        };

        var exception = Record.Exception(() => EnrollmentSelectionPolicy.Validate(subjects));

        Assert.Null(exception);
    }

    private static Subject CreateSubject(string name, Guid professorId)
    {
        return Subject.Create(Guid.NewGuid(), name, 3, professorId);
    }
}
