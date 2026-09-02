using Api.Modules.Identity.AccountManagerAssignments;
using Api.Modules.Identity.Auth;
using Api.Modules.Identity.Impersonation;
using Api.Modules.Identity.InternalUsers;
using Api.Modules.Identity.Roles;
using Api.Modules.Identity.SubAccounts;

namespace Api.Modules.Identity;

public static class IdentityEndpoints
{
    public static IEndpointRouteBuilder MapIdentityEndpoints(
        this IEndpointRouteBuilder app)
    {
        AuthEndpoints.Map(app);
        SubAccountEndpoints.Map(app);
        RoleEndpoints.Map(app);
        InternalUserEndpoints.Map(app);
        AccountManagerAssignmentEndpoints.Map(app);
        ImpersonationEndpoints.Map(app);

        return app;
    }
}

