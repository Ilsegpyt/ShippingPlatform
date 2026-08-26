using Identity.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Identity.Infrastructure.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public PermissionKey Permission { get; }

    public PermissionRequirement(PermissionKey permission) => Permission = permission;
}
