using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// Extends the standard ASP.NET Core Identity user. Kept intentionally thin —
/// business data (Scope, Permissions, OrganizationId for sub-accounts, etc.)
/// lives in Identity.Domain.SubAccount / Role, linked only by UserId as a
/// foreign key. This keeps the Authentication concern (Infrastructure) separate
/// from the Business concern (Domain).
/// </summary>
public sealed class ApplicationUser : IdentityUser<Guid>
{
    /// <summary>
    /// Distinguishes an internal employee account from a Customer/SubAccount login.
    /// Drives which portal's claims get issued at token time.
    /// </summary>
    public UserKind Kind { get; set; }

    public bool IsActive { get; set; } = true;
}

public enum UserKind
{
    Internal = 0,
    Customer = 1
}
