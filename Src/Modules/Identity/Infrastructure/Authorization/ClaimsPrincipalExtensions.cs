using System.Security.Claims;

namespace Identity.Infrastructure.Authorization;

public static class ClaimsPrincipalExtensions
{
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