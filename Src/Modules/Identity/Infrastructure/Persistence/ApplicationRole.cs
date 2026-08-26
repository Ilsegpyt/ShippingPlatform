using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Persistence;

/// <summary>
/// NOTE: This is the plain ASP.NET Core Identity role table (AspNetRoles), used only
/// as infrastructure plumbing if/when needed by Identity APIs. It is intentionally
/// NOT the same concept as Identity.Domain.Role (our business Role with Permissions).
/// Business role assignment/authorization always goes through Domain.Role, never this.
/// </summary>
public sealed class ApplicationRole : IdentityRole<Guid>
{
}
