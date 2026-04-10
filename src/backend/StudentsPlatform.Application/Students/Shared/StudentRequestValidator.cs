using StudentsPlatform.Application.Common.Exceptions;
using StudentsPlatform.Application.Common.Validation;

namespace StudentsPlatform.Application.Students.Shared;

internal static class StudentRequestValidator
{
    public static void ValidateProfile(string fullName, string email, string programName)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(fullName))
        {
            errors["fullName"] = ["El nombre completo es obligatorio."];
        }

        if (!EmailValidator.IsValid(email))
        {
            errors["email"] = ["Debes ingresar un correo valido."];
        }

        if (string.IsNullOrWhiteSpace(programName))
        {
            errors["programName"] = ["El programa academico es obligatorio."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    public static void ValidateSubjectIds(IReadOnlyCollection<Guid> subjectIds)
    {
        ArgumentNullException.ThrowIfNull(subjectIds);

        var errors = new Dictionary<string, string[]>();

        if (subjectIds.Any(id => id == Guid.Empty))
        {
            errors["subjectIds"] = ["La seleccion de materias contiene identificadores invalidos."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    public static void ValidatePasswordRequired(string? password)
    {
        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors["password"] = ["La contrasena es obligatoria."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }

    public static void ValidatePasswordOptional(string? password)
    {
        if (password is null)
        {
            return;
        }

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors["password"] = ["La contrasena no puede estar vacia."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }
}
