using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Ports.AddOrUpdatePortCoverage;

namespace Api.Modules.Transporters.Ports.AddOrUpdatePortCoverage;

public static class AddOrUpdatePortCoverageEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/transporters/{id:guid}/ports",
            async (
                Guid id,
                AddOrUpdatePortCoverageRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new AddOrUpdatePortCoverageCommand(
                    id,
                    request.PortName,
                    request.FreeHours,
                    request.DelayRatePerHour,
                    request.OfficialReceiptNotes,
                    request.GeneralNotes,
                    request.LaneRates ?? []);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersEdit);
    }
}

public sealed record AddOrUpdatePortCoverageRequest(
    string PortName,
    int FreeHours,
    decimal DelayRatePerHour,
    string? OfficialReceiptNotes,
    string? GeneralNotes,
    IReadOnlyList<LaneRateItem>? LaneRates);
