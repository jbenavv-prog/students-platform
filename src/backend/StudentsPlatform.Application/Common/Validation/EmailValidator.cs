using System.ComponentModel.DataAnnotations;

namespace StudentsPlatform.Application.Common.Validation;

internal static class EmailValidator
{
    private static readonly EmailAddressAttribute Attribute = new();

    public static bool IsValid(string? email)
    {
        return !string.IsNullOrWhiteSpace(email) && Attribute.IsValid(email);
    }
}
