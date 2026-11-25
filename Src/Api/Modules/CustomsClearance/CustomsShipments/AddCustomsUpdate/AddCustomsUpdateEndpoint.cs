using CustomsClearance.Application.CustomsShipments.AddCustomsUpdate;
using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;

namespace Api.Modules.CustomsClearance.CustomsShipments.AddCustomsUpdate;

public static class AddCustomsUpdateEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/api/customs-clearance/shipments/{id:guid}/updates",
            async (
                Guid id,
                AddCustomsUpdateRequest request,
                ISender sender,
                CancellationToken ct) =>
            {
                var command = new AddCustomsUpdateCommand(
                    id,
                    request.UpdateText);

                var result = await sender.Send(command, ct);

                return result.IsSuccess
                    ? Results.Ok()
                    : Results.BadRequest(result.Error);
            }).RequirePermission(PermissionCatalog.CustomsShipmentsEdit);
    }
}

public sealed record AddCustomsUpdateRequest(
    string UpdateText);
