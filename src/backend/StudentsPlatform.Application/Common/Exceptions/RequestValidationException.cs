namespace StudentsPlatform.Application.Common.Exceptions;

public sealed class RequestValidationException : Exception
{
    public RequestValidationException(IDictionary<string, string[]> errors, string message = "Request validation failed.")
        : base(message)
    {
        Errors = new Dictionary<string, string[]>(errors, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}
