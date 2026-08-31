using System.Security.Claims;

namespace Identity.Infrastructure.Authorization;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId =
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue("sub");

        if (!Guid.TryParse(userId, out var id))
        {
            throw new UnauthorizedAccessException(
                "User ID claim is missing or invalid.");
        }

        return id;
    }

    public static Guid GetOrganizationId(this ClaimsPrincipal user)
    {
        var organizationId = user.FindFirstValue("org_id");

        if (!Guid.TryParse(organizationId, out var id))
        {
            throw new UnauthorizedAccessException(
                "Organization ID claim is missing or invalid.");
        }

        return id;
    }
}