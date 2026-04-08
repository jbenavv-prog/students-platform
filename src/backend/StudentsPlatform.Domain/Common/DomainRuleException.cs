namespace StudentsPlatform.Domain.Common;

public sealed class DomainRuleException(string message) : Exception(message);
