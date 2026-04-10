using StudentsPlatform.Application.Common.Exceptions;
using StudentsPlatform.Application.Common.Validation;

namespace StudentsPlatform.Application.Auth.Login;

internal static class LoginRequestValidator
{
    public static void Validate(string email, string password)
    {
        var errors = new Dictionary<string, string[]>();

        if (!EmailValidator.IsValid(email))
        {
            errors["email"] = ["Debes ingresar un correo valido."];
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            errors["password"] = ["La contrasena es obligatoria."];
        }

        if (errors.Count > 0)
        {
            throw new RequestValidationException(errors);
        }
    }
}
