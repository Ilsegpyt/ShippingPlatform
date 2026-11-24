using BuildingBlocks.Application;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Transporters.GetTransporters;

namespace Api.Modules.Transporters.Transporters.GetTransporters;

public static class GetTransportersEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/transporters",
            async (
                [AsParameters] PaginationRequest pagination,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetTransportersQuery(
                    pagination);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersView);
    }
}
