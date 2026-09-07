using BuildingBlocks.Application;

namespace Identity.Application.Abstractions;

/// <summary>
/// Manages Identity users.
/// </summary>
public interface IIdentityUserService
{
    /// <summary>
    /// Creates a new Identity user.
    /// </summary>
    Task<Guid> CreateUserAsync(string email, string defaultPassword, bool isInternal, string? phone, CancellationToken ct = default);

    /// <summary>
    /// Validates user credentials.
    /// </summary>
    Task<Guid?> ValidateCredentialsAsync(string email, string password, CancellationToken ct = default);

    /// <summary>
    /// Activates or deactivates a user.
    /// </summary>
    Task SetUserStatusAsync(Guid userId, bool isActive, CancellationToken ct = default);

    /// <summary>
    /// Checks if a user is active.
    /// </summary>
    Task<bool> IsActiveAsync(Guid userId, CancellationToken ct = default);

    /// <summary>
    /// Updates the user's email.
    /// </summary>
    Task<IdentityUserOperationResult> UpdateEmailAsync(Guid userId, string email, CancellationToken ct = default);

    /// <summary>
    /// Resets the user's password.
    /// </summary>
    Task<IdentityUserOperationResult> ResetPasswordAsync(Guid userId, string newPassword, CancellationToken ct = default);

    /// <summary>
    /// Updates the user's password.
    /// </summary>
    Task<IdentityUserOperationResult> UpdatePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes the user.
    /// </summary>
    Task<IdentityUserOperationResult> DeleteUserAsync(Guid userId, CancellationToken ct = default);

}

public sealed record IdentityUserOperationResult(
    bool Succeeded,
    string? Error = null);