using System.Net.Mail;

namespace StudentsPlatform.Domain.Common;

public static class Guard
{
    public static string AgainstNullOrWhiteSpace(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{parameterName} is required.", parameterName);
        }

        return value.Trim();
    }

    public static string AgainstInvalidEmail(string? value, string parameterName)
    {
        var email = AgainstNullOrWhiteSpace(value, parameterName);

        try
        {
            var address = new MailAddress(email);
            if (!string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase))
            {
                throw new FormatException();
            }
        }
        catch (FormatException)
        {
            throw new ArgumentException($"{parameterName} must be a valid email address.", parameterName);
        }

        return email;
    }
}
