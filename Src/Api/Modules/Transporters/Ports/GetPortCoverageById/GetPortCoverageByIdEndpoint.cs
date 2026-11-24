using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Ports.GetPortCoverageById;

namespace Api.Modules.Transporters.Ports.GetPortCoverageById;

public static class GetPortCoverageByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/transporters/{id:guid}/ports/{portCoverageId:guid}",
            async (
                Guid id,
                Guid portCoverageId,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetPortCoverageByIdQuery(id, portCoverageId);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersView);
    }
}
