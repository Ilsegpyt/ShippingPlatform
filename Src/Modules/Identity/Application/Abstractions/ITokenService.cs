namespace Identity.Application.Abstractions;

/// <summary>
/// Represents an access token and its corresponding refresh token.
/// </summary>
public sealed record TokenPair(
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAtUtc);

/// <summary>
/// Handles issuing, refreshing, and revoking authentication tokens.
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Issues a new access and refresh token pair.
    /// </summary>
    Task<TokenPair> IssueTokensAsync(Guid userId, IReadOnlyDictionary<string, string> claims, CancellationToken ct = default);

    /// <summary>
    /// Issues a new token pair for an impersonation session.
    /// </summary>
    Task<TokenPair> IssueImpersonationTokensAsync(Guid impersonatorUserId, Guid impersonatedOrganizationId, CancellationToken ct = default);

    /// <summary>
    /// Validates and rotates a refresh token.
    /// Returns null if the token is invalid, expired, or revoked.
    /// </summary>
    Task<TokenPair?> RefreshAsync(string refreshToken, CancellationToken ct = default);

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    Task RevokeAsync(string refreshToken, CancellationToken ct = default);
}
