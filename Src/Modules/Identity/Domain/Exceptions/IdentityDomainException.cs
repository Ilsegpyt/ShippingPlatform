namespace Identity.Domain.Exceptions;

/// <summary>
/// Base type for business-rule violations raised from within the Identity domain model.
/// Reserved for truly invalid state transitions / invariant violations — NOT for
/// ordinary validation (that belongs in Application-layer Validators).
/// </summary>
public abstract class IdentityDomainException : Exception
{
    protected IdentityDomainException(string message) : base(message) { }
}

public sealed class InvalidScopeCombinationException : IdentityDomainException
{
    public InvalidScopeCombinationException(string message) : base(message) { }
}

public sealed class InvalidSubAccountStateException : IdentityDomainException
{
    public InvalidSubAccountStateException(string message) : base(message) { }
}
