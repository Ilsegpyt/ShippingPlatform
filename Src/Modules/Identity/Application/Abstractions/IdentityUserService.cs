using BuildingBlocks.Application;

namespace Identity.Application.Abstractions;

/// <summary>
/// Manages the underlying ASP.NET Core Identity users.
/// </summary>
public interface IIdentityUserService
{
    /// <summary>
    /// Creates an Identity user with a temporary/default password.
    /// </summary>
    Task<Guid> CreateUserAsync(
        string email,
        string defaultPassword,
        bool isInternal,
        string? phone,
        CancellationToken ct = default);

    /// <summary>
    /// Validates user credentials and returns the user ID if valid.
    /// </summary>
    Task<Guid?> ValidateCredentialsAsync(
        string email,
        string password,
        CancellationToken ct = default);

    /// <summary>
    /// Activates or deactivates an Identity user.
    /// </summary>
    Task SetActiveAsync(
        Guid userId,
        bool isActive,
        CancellationToken ct = default);

    /// <summary>
    /// Checks whether an Identity user is active.
    /// </summary>
    Task<bool> IsActiveAsync(
        Guid userId,
        CancellationToken ct = default);


    Task<Result> UpdateEmailAsync(
    Guid userId,
    string email,
    CancellationToken ct = default);

    Task<Result> ResetPasswordAsync(
   Guid userId,
   string newPassword,
   CancellationToken ct = default);


    Task<Result> DeleteUserAsync(
    Guid userId,
    CancellationToken ct = default);

}

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
    Task<TokenPair> IssueTokensAsync(
        Guid userId,
        IReadOnlyDictionary<string, string> claims,
        CancellationToken ct = default);

    /// <summary>
    /// Validates and rotates a refresh token.
    /// Returns null if the token is invalid, expired, or revoked.
    /// </summary>
    Task<TokenPair?> RefreshAsync(
        string refreshToken,
        CancellationToken ct = default);

    /// <summary>
    /// Revokes a refresh token.
    /// </summary>
    Task RevokeAsync(
        string refreshToken,
        CancellationToken ct = default);

   
}