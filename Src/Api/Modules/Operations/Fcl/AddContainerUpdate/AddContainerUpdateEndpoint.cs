using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.AddContainerUpdate;

namespace Api.Modules.Operations.AddContainerUpdate;

public static class AddContainerUpdateEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/operations/containers/{containerId:guid}/updates", async (
            Guid containerId,
            AddContainerUpdateRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new AddContainerUpdateCommand(containerId, request.Details), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.OperationsEdit);
    }
}

public sealed record AddContainerUpdateRequest(string Details);
