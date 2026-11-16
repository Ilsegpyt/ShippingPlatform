using Identity.Domain.ValueObjects;
using Identity.Infrastructure.Authorization;
using MediatR;
using Operations.Application.Operations.DeliverOperation;

namespace Api.Modules.Operations.DeliverOperation;

public static class DeliverOperationEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    {
        app.MapPut("/api/operations/{operationId:guid}/deliver", async (
            Guid operationId,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(
                new DeliverOperationCommand(operationId), ct);

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.BadRequest(result.Error);
        }).RequirePermission(PermissionCatalog.OperationsDeliver);
    }
}
