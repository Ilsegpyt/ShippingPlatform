using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.GetAllFclApprovals;

namespace Api.Modules.Operations.GetAllFclApprovals;

public static class GetAllFclApprovalsEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/fcl-approvals",
            async (
                ISender sender,
                CancellationToken ct) =>
            {
                var result = await sender.Send(
                    new GetAllFclApprovalsQuery(),
                    ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(
                        new { Error = result.Error });
            })
            .RequirePermission(PermissionCatalog.OperationsView);
    }
}