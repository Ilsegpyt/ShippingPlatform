using Identity.Domain;
using Microsoft.AspNetCore.Builder;


namespace Identity.Infrastructure.Authorization;

public static class EndpointAuthorizationExtensions
{
    /// <summary>
    /// Attaches a permission check to a Minimal API endpoint, e.g.:
    /// app.MapPost(...).RequirePermission(PermissionCatalog.ShipmentsEdit);
    /// The actual check happens in PermissionAuthorizationHandler, live against the database.
    /// </summary>
    /// 
    //public static TBuilder RequirePermission<TBuilder>(this TBuilder builder, PermissionKey permission)
    //    where TBuilder : IEndpointConventionBuilder
    //{
    //    builder.RequireAuthorization(policy => policy.Requirements.Add(new PermissionRequirement(permission)));
    //    return builder;
    //}
    public static TBuilder RequirePermission<TBuilder>(
        this TBuilder builder,
        PermissionKey permission)
        where TBuilder : IEndpointConventionBuilder
    {
        builder.RequireAuthorization(policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.Requirements.Add(
                new PermissionRequirement(permission));
        });

        return builder;
    }
}