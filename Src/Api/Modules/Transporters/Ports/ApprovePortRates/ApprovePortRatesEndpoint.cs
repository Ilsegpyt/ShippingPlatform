using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Ports.ApprovePortRates;

namespace Api.Modules.Transporters.Ports.ApprovePortRates;

public static class ApprovePortRatesEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/transporters/ports/{portCoverageId:guid}/approve",
            async (
                Guid portCoverageId,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new ApprovePortRatesCommand(portCoverageId);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersApproveRates);
    }
}
