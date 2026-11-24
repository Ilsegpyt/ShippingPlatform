using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Transporters.Application.Transporters.GetTransporterById;

namespace Api.Modules.Transporters.Transporters.GetTransporterById;

public static class GetTransporterByIdEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapGet(
            "/api/transporters/{id:guid}",
            async (
                Guid id,
                ISender sender,
                CancellationToken ct) =>
            {
                var query = new GetTransporterByIdQuery(id);

                var result = await sender.Send(query, ct);

                return result.IsSuccess
                    ? Results.Ok(result.Value)
                    : Results.NotFound(result.Error);
            }).RequirePermission(PermissionCatalog.TransportersView);
    }
}
