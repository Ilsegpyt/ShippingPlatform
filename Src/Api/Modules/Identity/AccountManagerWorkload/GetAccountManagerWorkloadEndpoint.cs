using Identity.Application.AccountManagerWorkload;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.Identity.AccountManagerWorkload;

public static class GetAccountManagerWorkloadEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/identity/account-manager-workload",
                async (
                    ISender sender,
                    CancellationToken ct) =>
                {
                    var query =
                        new GetAccountManagerWorkloadQuery();

                    var result =
                        await sender.Send(query, ct);

                    if (result.IsFailure)
                        return Results.BadRequest(result.Error);

                    return Results.Ok(result.Value);
                })
            .RequirePermission(
                PermissionCatalog.AccountManagerWorkloadView);
    }
}