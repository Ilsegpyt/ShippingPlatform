namespace Identity.Infrastructure.Persistence;

/// <summary>
/// Infrastructure-level concern (not part of the Domain model) supporting the
/// JWT Access Token + Refresh Token strategy, with rotation: each time a refresh
/// token is used it is revoked and replaced, reducing the blast radius of a stolen token.
/// </summary>
public sealed class RefreshToken
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    /// <summary>
    /// Stored as a hash, never plain text.
    /// </summary>
    public string TokenHash { get; private set; } = null!;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public Guid? ReplacedByTokenId { get; private set; }

    public string TokenType { get; private set; } = null!;

    public Guid? ImpersonatedOrganizationId { get; private set; }

    public bool IsActive =>
        RevokedAtUtc is null &&
        DateTime.UtcNow < ExpiresAtUtc;

    private RefreshToken()
    {
    }

    /// <summary>
    /// Creates a normal refresh token.
    /// </summary>
    public static RefreshToken Create(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            TokenType = "normal",
            ImpersonatedOrganizationId = null
        };
    }

    /// <summary>
    /// Creates a refresh token for an impersonation session.
    /// </summary>
    public static RefreshToken CreateImpersonation(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc,
        Guid impersonatedOrganizationId)
    {
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = tokenHash,
            ExpiresAtUtc = expiresAtUtc,
            CreatedAtUtc = DateTime.UtcNow,
            TokenType = "impersonation",
            ImpersonatedOrganizationId = impersonatedOrganizationId
        };
    }

    public void Revoke(Guid? replacedByTokenId = null)
    {
        RevokedAtUtc = DateTime.UtcNow;
        ReplacedByTokenId = replacedByTokenId;
    }
}